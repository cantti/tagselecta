using System.Threading.Channels;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.Commands.Tui.Widgets;
using TagSelecta.Shared.IO;

namespace TagSelecta.Commands.Tui;

public class TuiApp(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    HotkeyMap hotkeys,
    IRequestReader requestReader,
    ITuiCommandFactory commandFactory,
    ITagDataActionTargetFactory tagDataActionTargetFactory
) : AsyncCommand<TuiSettings>, ITuiCommandContext
{
    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";
    private const string CommandLayoutKey = "command";
    private const string StatusLayoutKey = "status";

    private string _statusMessage = "";

    public IEnumerable<TagDataActionTarget> Files { get; private set; } = [];

    public IEnumerable<TagDataActionTarget> VisibleFiles =>
        Files.Where(x => !FilterEnabled || x.HasChanges);

    public IEnumerable<TagDataActionTarget> SelectedFiles =>
        VisibleFiles.Any(x => x.IsSelected) ? VisibleFiles.Where(x => x.IsSelected)
        : FocusedFile is not null ? new[] { FocusedFile }
        : Enumerable.Empty<TagDataActionTarget>();

    public int FocusedFileIndex { get; set; }

    public TagDataActionTarget? FocusedFile => VisibleFiles.ElementAtOrDefault(FocusedFileIndex);

    private CancellationTokenSource _cts = new();

    private CancellationTokenSource? _currentCommandCts;
    private Task _currentCommandTask = Task.CompletedTask;

    public bool TreeEnabled { get; set; }
    public bool FilterEnabled { get; set; }
    public bool HelpEnabled { get; set; }

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        TuiSettings settings,
        CancellationToken ct
    )
    {
        try
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            BindHotkeys();

            if (!ValidateOptions(context))
            {
                return 1;
            }

            AltScreen.Enter();

            Files = audioFileScanner
                .SearchAndRead(settings.Path, ct)
                .Select(x => tagDataActionTargetFactory.Create(x.Path, x.TagData))
                .ToList();

            var channel = Channel.CreateUnbounded<ConsoleKeyInfo>();
            _ = StartInputLoop(channel);
            await StartUiLoop(channel);
        }
        catch (OperationCanceledException)
        {
            // expected
        }
        finally
        {
            AltScreen.Exit();
        }

        return 0;
    }

    private Task StartUiLoop(Channel<ConsoleKeyInfo> channel)
    {
        return console
            .Live(new Panel("Starting..."))
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    ctx.UpdateTarget(DrawLayout());
                    while (channel.Reader.TryRead(out var key))
                    {
                        if (requestReader.TryRead(key, out var request))
                        {
                            await DispatchCommand(request);
                        }
                        ctx.UpdateTarget(DrawLayout());
                    }
                    await Task.Delay(33, _cts.Token);
                }
            });
    }

    private Task StartInputLoop(Channel<ConsoleKeyInfo> channel)
    {
        return Task.Run(() =>
        {
            try
            {
                while (true)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (_currentCommandTask.IsCompleted || key.Key == ConsoleKey.Escape)
                    {
                        channel.Writer.TryWrite(key);
                    }
                }
            }
            catch
            {
                // ignored
            }
        });
    }

    private IRenderable DrawLayout()
    {
        var filesContentSize = (Console.WindowHeight - 3 - 1 - 1 - 1) / 2;

        var layout = new Layout("root").SplitRows(
            new Layout(HeaderLayoutKey).Size(3).Update(RenderHeader()),
            new Layout(FilesLayoutKey)
                .Size(filesContentSize)
                .Update(
                    HelpEnabled ? new HelpWidget()
                    : TreeEnabled
                        ? new TreeListWidget(VisibleFiles, FocusedFile, filesContentSize - 2)
                    : new FileListWidget(VisibleFiles, FocusedFile, filesContentSize - 2)
                ),
            new Layout(TagDataLayoutKey).Ratio(1).Update(new TagDataWidget(FocusedFile)),
            new Layout(StatusLayoutKey).Size(1).Update(new StatusWidget(_statusMessage)),
            new Layout(CommandLayoutKey)
                .Size(1)
                .Update(
                    requestReader.Mode == InputMode.Command
                        ? new CommandPromptWidget(requestReader.Text, requestReader.CursorPos)
                        : Text.Empty
                )
        );
        return layout;
    }

    private bool ValidateOptions(CommandContext context)
    {
        var unknownOptions = context
            .Remaining.Parsed.Select(kvp => kvp.Key)
            .Concat(context.Remaining.Raw)
            .ToList();
        if (unknownOptions.Count != 0)
        {
            console.MarkupLine($"[red]Unknown option(s) provided:[/]");
            foreach (var option in unknownOptions)
            {
                console.WriteLine($"  {option}", new Style(Color.Yellow));
            }
            return false;
        }
        return true;
    }

    private IRenderable RenderHeader()
    {
        var keys = new List<(string Key, string Action)> { ("q", "Quit"), ("h", "Help") };
        var cols1 = keys.Select(x => new Markup($"[bold blue]{x.Key}[/] ➔ {x.Action}")).ToList();
        return new Rows(
            new Text("Tagselecta:", new Style(Color.Yellow)),
            new Columns(cols1) { Padding = new Padding(2, 0, 2, 0), Expand = false }
        );
    }

    public void Quit()
    {
        _cts.Cancel();
    }

    private readonly Lock _printLock = new();

    public void Print(string markupMessage)
    {
        lock (_printLock)
        {
            _statusMessage = markupMessage;
        }
    }

    private void BindHotkeys()
    {
        hotkeys.Bind(HotkeyTokens.Esc, "clearselection");
        hotkeys.Bind(HotkeyTokens.Down, "movedown");
        hotkeys.Bind(HotkeyTokens.Up, "moveup");
        hotkeys.Bind("j", "movedown");
        hotkeys.Bind("k", "moveup");
        hotkeys.Bind("g", "movestart");
        hotkeys.Bind("G", "moveend");
        hotkeys.Bind("q", "quit");
        hotkeys.Bind("t", "toggletree");
        hotkeys.Bind("f", "togglefilter");
        hotkeys.Bind("h", "togglehelp");
        hotkeys.Bind("u", "undo");
        hotkeys.Bind(HotkeyTokens.Tab, "select");
        hotkeys.Bind(HotkeyTokens.Space, "select");
        hotkeys.Bind("a", "selectall");
        hotkeys.Bind("*", "selectall");
        hotkeys.Bind("w", "write");
    }

    private async Task DispatchCommand(Request request)
    {
        var isCancelRequest = request.Name == "cancel";

        if (isCancelRequest && _currentCommandCts is not null)
        {
            await _currentCommandCts.CancelAsync();
            _currentCommandCts.Dispose();
        }

        await _currentCommandTask;

        if (isCancelRequest)
        {
            return;
        }

        _currentCommandCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);

        var command = commandFactory.Create(request.Name);

        hotkeys.Bind("esc", "cancel");

        _currentCommandTask = Task.Run(async () =>
        {
            try
            {
                await command.ExecuteAsync(this, request, _currentCommandCts.Token);
            }
            catch (OperationCanceledException)
            {
                Print("Cancelled.");
            }
            catch (Exception ex)
            {
                Print(ex.Message);
            }
            hotkeys.Bind("esc", "clearselection");
        });
    }
}
