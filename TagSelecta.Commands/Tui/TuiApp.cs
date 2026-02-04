using System.Threading.Channels;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.Commands.Tui.Widgets;
using TagSelecta.Shared.IO;

namespace TagSelecta.Commands.Tui;

public class TuiApp : AsyncCommand<TuiSettings>, ITuiCommandContext
{
    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";
    private const string CommandLayoutKey = "command";
    private const string StatusLayoutKey = "status";
    private string _statusMessage = "";
    private CancellationTokenSource? _currentCommandCts;
    private CancellationTokenSource _cts = new();
    private Task _currentCommandTask = Task.CompletedTask;

    public TuiApp(
        IAnsiConsole console,
        IAudioFileScanner audioFileScanner,
        ITuiCommandFactory commandFactory,
        ITagDataActionTargetFactory tagDataActionTargetFactory
    )
    {
        _console = console;
        _audioFileScanner = audioFileScanner;
        _commandFactory = commandFactory;
        _tagDataActionTargetFactory = tagDataActionTargetFactory;
        _hotkeys = new HotkeyMap();
        _requestReader = new RequestReader(_hotkeys);
    }

    public TagDataActionTarget? FocusedFile => VisibleFiles.ElementAtOrDefault(FocusedFileIndex);

    public IEnumerable<TagDataActionTarget> Files { get; private set; } = [];

    public IEnumerable<TagDataActionTarget> VisibleFiles =>
        Files.Where(x => !FilterEnabled || x.HasChanges);

    public IEnumerable<TagDataActionTarget> SelectedFiles =>
        VisibleFiles.Any(x => x.IsSelected) ? VisibleFiles.Where(x => x.IsSelected)
        : FocusedFile is not null ? new[] { FocusedFile }
        : Enumerable.Empty<TagDataActionTarget>();

    public int FocusedFileIndex { get; set; }

    public bool TreeEnabled { get; set; }

    public bool FilterEnabled { get; set; }

    public bool KeymapHelpEnabled { get; set; }

    public bool CommandHelpEnabled { get; set; }

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

            Files = _audioFileScanner
                .SearchAndRead(settings.Path, ct)
                .Select(x => _tagDataActionTargetFactory.Create(x.Path, x.TagData))
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
        return _console
            .Live(new Panel("Starting..."))
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    ctx.UpdateTarget(DrawLayout());
                    while (channel.Reader.TryRead(out var key))
                    {
                        if (_requestReader.TryRead(key, out var request))
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
        var children = new List<Layout>();
        children.Add(new Layout(HeaderLayoutKey).Size(3).Update(RenderHeader()));
        if (KeymapHelpEnabled || CommandHelpEnabled)
        {
            children.Add(
                new Layout(FilesLayoutKey).Update(
                    KeymapHelpEnabled ? new KeymapHelpWidget()
                    : CommandHelpEnabled ? new CommandHelpWidget()
                    : Text.Empty
                )
            );
        }
        else
        {
            var filesContentSize = (Console.WindowHeight - 3 - 1 - 1 - 1) / 2;
            children.Add(
                new Layout(FilesLayoutKey)
                    .Size(filesContentSize)
                    .Update(
                        TreeEnabled
                            ? new TreeListWidget(VisibleFiles, FocusedFile, filesContentSize - 2)
                            : new FileListWidget(VisibleFiles, FocusedFile, filesContentSize - 2)
                    )
            );
            children.Add(
                new Layout(TagDataLayoutKey).Ratio(1).Update(new TagDataWidget(FocusedFile))
            );
        }
        children.Add(new Layout(StatusLayoutKey).Size(1).Update(new StatusWidget(_statusMessage)));
        children.Add(
            new Layout(CommandLayoutKey)
                .Size(1)
                .Update(
                    _requestReader.Mode == InputMode.Command
                        ? new CommandPromptWidget(_requestReader.Text, _requestReader.CursorPos)
                        : Text.Empty
                )
        );
        var layout = new Layout("root").SplitRows(children.ToArray());
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
            _console.MarkupLine($"[red]Unknown option(s) provided:[/]");
            foreach (var option in unknownOptions)
            {
                _console.WriteLine($"  {option}", new Style(Color.Yellow));
            }
            return false;
        }
        return true;
    }

    private IRenderable RenderHeader()
    {
        var keys = new List<(string Key, string Action)>
        {
            ("q", "Quit"),
            ("h", "Command help"),
            ("?", "Keymap help"),
        };
        var cols1 = new List<IRenderable>();
        cols1.AddRange(
            keys.Select(x => new Markup($"[bold blue]{x.Key}[/] ➔ {x.Action}")).ToList()
        );
        cols1.Add(
            new Markup("[bold blue]Documentation[/]: [link]https://github.com/cantti/tagselecta[/]")
        );
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
    private readonly IAnsiConsole _console;
    private readonly IAudioFileScanner _audioFileScanner;
    private readonly HotkeyMap _hotkeys;
    private readonly ITuiCommandFactory _commandFactory;
    private readonly ITagDataActionTargetFactory _tagDataActionTargetFactory;
    private readonly RequestReader _requestReader;

    public void Print(string markupMessage)
    {
        lock (_printLock)
        {
            _statusMessage = markupMessage;
        }
    }

    private void BindHotkeys()
    {
        _hotkeys.Bind(HotkeyTokens.Esc, "clearselection");
        _hotkeys.Bind(HotkeyTokens.Down, "movedown");
        _hotkeys.Bind(HotkeyTokens.Up, "moveup");
        _hotkeys.Bind("j", "movedown");
        _hotkeys.Bind("k", "moveup");
        _hotkeys.Bind("g", "movestart");
        _hotkeys.Bind("G", "moveend");
        _hotkeys.Bind("q", "quit");
        _hotkeys.Bind("t", "toggletree");
        _hotkeys.Bind("f", "togglefilter");
        _hotkeys.Bind("?", "togglekeymaphelp");
        _hotkeys.Bind("h", "togglecommandhelp");
        _hotkeys.Bind("u", "undo");
        _hotkeys.Bind(HotkeyTokens.Tab, "select");
        _hotkeys.Bind(HotkeyTokens.Space, "select");
        _hotkeys.Bind("a", "selectall");
        _hotkeys.Bind("A", "selectdir");
        _hotkeys.Bind("*", "selectall");
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

        var command = _commandFactory.Create(request.Name);

        _hotkeys.Bind("esc", "cancel");

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
            _hotkeys.Bind("esc", "clearselection");
        });
    }
}
