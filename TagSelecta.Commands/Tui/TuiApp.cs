using System.Threading.Channels;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Commands.Tui.Completion;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.Commands.Tui.Widgets;
using TagSelecta.Shared.IO;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui;

public class TuiApp(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITuiCommandFactory commandFactory,
    TuiAppConfig config,
    InputHandler inputHandler,
    HotkeyMap hotkeys,
    ICompletionProvider completionProvider
) : AsyncCommand<TuiSettings>, ITuiCommandContext
{
    private readonly Lock _printLock = new();
    private int _isDirty = 1;
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

    public bool TreeEnabled { get; set; } = config.TreeEnabled;

    public bool FilterEnabled { get; set; }

    public bool KeymapHelpEnabled { get; set; }

    public bool CommandHelpEnabled { get; set; }

    public bool FileListEnabled { get; set; } = true;

    public bool PictureEnabled { get; set; }

    public void SetCommandPromptText(string text)
    {
        inputHandler.SetText(text);
        Invalidate();
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

        Invalidate();
    }

    public void SetFiles(IEnumerable<TagDataActionTarget> files)
    {
        Files = files.ToList();
        completionProvider.GenerateCompletions(
            Files.SelectMany(x => x.CurrentTagData.Fields).Select(x => x.Key)
        );
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TuiSettings settings,
        CancellationToken ct
    )
    {
        try
        {
            inputHandler.IsAutoCompletionEnabled = config.AutoCompletionEnabled;
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

            completionProvider.GenerateCompletions(
                Files.SelectMany(x => x.CurrentTagData.Fields).Select(x => x.Key)
            );

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
        const int activeTickMs = 16;
        const int idleTickMs = 200;
        const int activeWindowMs = 300;

        var lastConsoleWidth = Console.WindowWidth;
        var lastConsoleHeight = Console.WindowHeight;

        return console
            .Live(new Panel("Starting..."))
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                var activeUntil = DateTime.UtcNow;

                while (!_cts.Token.IsCancellationRequested)
                {
                    var hadInput = false;
                    while (channel.Reader.TryRead(out var key))
                    {
                        hadInput = true;

                        if (inputHandler.ProcessKey(key, out var request))
                        {
                            await DispatchCommand(request);
                        }
                    }

                    if (hadInput)
                    {
                        activeUntil = DateTime.UtcNow.AddMilliseconds(activeWindowMs);
                        Invalidate();
                    }
                    else if (
                        lastConsoleWidth != Console.WindowWidth
                        || lastConsoleHeight != Console.WindowHeight
                    )
                    {
                        lastConsoleWidth = Console.WindowWidth;
                        lastConsoleHeight = Console.WindowHeight;
                        Invalidate();
                    }

                    if (TryConsumeDirty())
                    {
                        ctx.UpdateTarget(DrawLayout());
                    }

                    var isActive = DateTime.UtcNow < activeUntil;
                    var delayTask = Task.Delay(isActive ? activeTickMs : idleTickMs, _cts.Token);
                    var waitForInputTask = channel.Reader.WaitToReadAsync(_cts.Token).AsTask();
                    await Task.WhenAny(delayTask, waitForInputTask);
                }
            });
    }

    private void Invalidate()
    {
        Interlocked.Exchange(ref _isDirty, 1);
    }

    private bool TryConsumeDirty()
    {
        return Interlocked.Exchange(ref _isDirty, 0) == 1;
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
        const string headerLayoutKey = "navigation";
        const string tagDataLayoutKey = "body";
        const string statusLayoutKey = "status";
        const string filesLayoutKey = "files";
        const int headerSize = 3;
        var actualHeaderSize = 0;
        const int statusBarHeight = 1;
        var children = new List<Layout>();
        if (config.HeaderVisible)
        {
            actualHeaderSize = headerSize;
            children.Add(new Layout(headerLayoutKey).Size(actualHeaderSize).Update(RenderHeader()));
        }
        if (KeymapHelpEnabled || CommandHelpEnabled || PictureEnabled)
        {
            children.Add(
                new Layout(filesLayoutKey).Update(
                    KeymapHelpEnabled ? new KeymapHelpWidget()
                    : CommandHelpEnabled ? new CommandHelpWidget()
                    : PictureEnabled
                        ? new PictureWidget(
                            FocusedFile,
                            console.Profile.Height - actualHeaderSize - statusBarHeight
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
                    (Console.WindowHeight - actualHeaderSize - statusBarHeight - fileListPadding)
                    * config.FileListRatio
                );
                if (filesContentHeight < 1)
                {
                    // min size supported by spectre layout
                    filesContentHeight = 1;
                }

                children.Add(
                    new Layout(filesLayoutKey)
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
                new Layout(tagDataLayoutKey).Ratio(1).Update(new TagDataWidget(FocusedFile))
            );
        }

        children.Add(
            new Layout(statusLayoutKey)
                .Size(statusBarHeight)
                .Update(
                    inputHandler.Mode == InputMode.Command
                        ? new CommandPromptWidget(
                            inputHandler.Input,
                            inputHandler.CursorPos,
                            inputHandler.Completion
                        )
                        : new StatusWidget(_statusMessage)
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
        cols1.AddRange(keys.Select(x => new Markup($"[bold blue]{x.Key}[/] {x.Action}")).ToList());
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
        hotkeys.Bind(HotkeyTokens.Esc, "escape");
        hotkeys.Bind(HotkeyTokens.Down, "movedown");
        hotkeys.Bind(HotkeyTokens.Up, "moveup");
        hotkeys.Bind("j", "movedown");
        hotkeys.Bind("k", "moveup");
        hotkeys.Bind("g", "movestart");
        hotkeys.Bind("G", "moveend");
        hotkeys.Bind("q", "quit");
        hotkeys.Bind("t", "toggletree");
        hotkeys.Bind("f", "togglefilter");
        hotkeys.Bind("?", "togglekeymaphelp");
        hotkeys.Bind("h", "togglecommandhelp");
        hotkeys.Bind("H", "opendocs");
        hotkeys.Bind("e", "togglefilelist");
        hotkeys.Bind("u", "undo");
        hotkeys.Bind(HotkeyTokens.Tab, "select");
        hotkeys.Bind(HotkeyTokens.Space, "select");
        hotkeys.Bind("a", "selectall");
        hotkeys.Bind("A", "selectdir");
        hotkeys.Bind("*", "selectall");
        hotkeys.Bind("p", "togglepicture");
        hotkeys.Bind("P", "openpicture");
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

        hotkeys.Bind("esc", "cancel");

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
            finally
            {
                hotkeys.Bind("esc", "escape");
                Invalidate();
            }
        });
    }
}
