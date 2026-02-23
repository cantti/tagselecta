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
    ITuiCommandFactory commandFactory,
    TuiAppConfig config,
    InputHandler inputHandler
) : AsyncCommand<TuiSettings>, ITuiCommandContext
{
    private readonly HotkeyMap _hotkeys = new();
    private readonly Lock _printLock = new();
    private CancellationTokenSource _cts = new();
    private CancellationTokenSource? _currentCommandCts;
    private Task _currentCommandTask = Task.CompletedTask;
    private string _statusMessage = "";

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

    public bool FileListEnabled { get; set; } = true;

    public bool PictureEnabled { get; set; }

    public void SetCommandPromptText(string text)
    {
        inputHandler.SetText(text);
    }

    public void Quit()
    {
        _cts.Cancel();
    }

    public void Print(string markupMessage)
    {
        lock (_printLock)
        {
            _statusMessage = markupMessage;
        }
    }

    public override async Task<int> ExecuteAsync(
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
                .Select(x => new TagDataActionTarget(x.Path, x.TagData))
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
                        if (inputHandler.ProcessKey(key, out var request))
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
                    var key = Console.ReadKey(true);
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
        const string HeaderLayoutKey = "navigation";
        const string TagDataLayoutKey = "body";
        const string CommandLayoutKey = "command";
        const string StatusLayoutKey = "status";
        const string FilesLayoutKey = "files";
        const int headerSize = 3;
        const int statusBarHeight = 1;
        const int commandBarHeight = 1;
        var children = new List<Layout>();
        children.Add(new Layout(HeaderLayoutKey).Size(headerSize).Update(RenderHeader()));
        if (KeymapHelpEnabled || CommandHelpEnabled || PictureEnabled)
        {
            children.Add(
                new Layout(FilesLayoutKey).Update(
                    KeymapHelpEnabled ? new KeymapHelpWidget()
                    : CommandHelpEnabled ? new CommandHelpWidget()
                    : PictureEnabled
                        ? new PictureWidget(
                            FocusedFile,
                            console.Profile.Height - headerSize - statusBarHeight - commandBarHeight
                        )
                    : Text.Empty
                )
            );
        }
        else
        {
            if (FileListEnabled)
            {
                const int fileListPadding = 1;
                var filesContentHeight = (int)(
                    (
                        Console.WindowHeight
                        - headerSize
                        - statusBarHeight
                        - commandBarHeight
                        - fileListPadding
                    ) * config.FileListRatio
                );
                children.Add(
                    new Layout(FilesLayoutKey)
                        .Size(filesContentHeight)
                        .Update(
                            TreeEnabled
                                ? new TreeListWidget(
                                    VisibleFiles,
                                    FocusedFile,
                                    filesContentHeight - 2
                                )
                                : new FileListWidget(
                                    VisibleFiles,
                                    FocusedFile,
                                    filesContentHeight - 2
                                )
                        )
                );
            }

            children.Add(
                new Layout(TagDataLayoutKey).Ratio(1).Update(new TagDataWidget(FocusedFile))
            );
        }

        children.Add(
            new Layout(StatusLayoutKey)
                .Size(statusBarHeight)
                .Update(new StatusWidget(_statusMessage))
        );
        children.Add(
            new Layout(CommandLayoutKey)
                .Size(statusBarHeight)
                .Update(
                    inputHandler.Mode == InputMode.Command
                        ? new CommandPromptWidget(
                            inputHandler.Text,
                            inputHandler.CursorPos,
                            inputHandler.Completion
                        )
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
            console.MarkupLine("[red]Unknown option(s) provided:[/]");
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
            new Markup("[bold blue]Documentation[/]: [link]https://cantti.github.io/tagselecta[/]")
        );
        return new Rows(
            new SectionHeaderWidget("Tagselecta"),
            new Columns(cols1) { Padding = new Padding(2, 0, 2, 0), Expand = false }
        );
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
        _hotkeys.Bind("H", "opendocs");
        _hotkeys.Bind("e", "togglefilelist");
        _hotkeys.Bind("u", "undo");
        _hotkeys.Bind(HotkeyTokens.Tab, "select");
        _hotkeys.Bind(HotkeyTokens.Space, "select");
        _hotkeys.Bind("a", "selectall");
        _hotkeys.Bind("A", "selectdir");
        _hotkeys.Bind("*", "selectall");
        _hotkeys.Bind("p", "togglepicture");
        _hotkeys.Bind("P", "openpicture");
    }

    private async Task DispatchCommand(string commandText)
    {
        var isCancelRequest = commandText == "cancel";

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

        if (!CommandParser.TryParse(commandText, out var commands))
        {
            Print($"Invalid command: {commandText}");
            return;
        }

        _hotkeys.Bind("esc", "cancel");

        _currentCommandTask = Task.Run(async () =>
        {
            try
            {
                foreach (var parsedCommand in commands)
                {
                    _currentCommandCts.Token.ThrowIfCancellationRequested();
                    var command = commandFactory.Create(parsedCommand.Name);
                    await command.ExecuteAsync(this, parsedCommand, _currentCommandCts.Token);
                }
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
