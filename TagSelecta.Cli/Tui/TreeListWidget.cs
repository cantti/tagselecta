using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Tui;

public class TreeListWidget(
    List<TagDataOperation> operations,
    int selectedOperationIndex,
    int windowSize
) : Renderable
{
    private readonly StringComparison _pathComparer = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable content;

        if (operations.Count == 0)
        {
            content = Text.Empty;
            return content.Render(options, maxWidth);
        }

        var treeLines = BuildTreeLines();

        var selectedOperation = operations[selectedOperationIndex];

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var selectedIndexInTree = treeLines.FindIndex(x => x.Operation == selectedOperation);
        var windowStart = selectedIndexInTree - (windowSize / 2);

        // clamp so we dont go before 0 or past the last possible full window start
        var maxStart = Math.Max(0, treeLines.Count - windowSize);
        windowStart = Math.Clamp(windowStart, 0, maxStart);

        var linesToPrint = Math.Min(windowSize, treeLines.Count - windowStart);

        var items = new List<IRenderable>();

        for (var i = 0; i < linesToPrint; i++)
        {
            var itemIndex = windowStart + i;
            var treeLine = treeLines[itemIndex];
            items.Add(
                new Markup(
                    treeLine.Markup,
                    new Style(
                        null,
                        treeLine.Operation == selectedOperation ? Color.Gray : Color.Default
                    )
                )
            );
        }

        content = new Rows(new Text($"Files:", new Style(Color.Yellow)), new Rows(items));

        return content.Render(options, maxWidth);
    }

    private List<TreeLine> BuildTreeLines()
    {
        var treeLines = new List<TreeLine>();

        var paths = new List<(string Path, TagDataOperation? operation)>();

        foreach (var operation in operations)
        {
            paths.Add((operation.OriginalPath, operation));

            var current = operation.OriginalPath;
            var root = Path.GetPathRoot(current);

            while (true)
            {
                var parent = Path.GetDirectoryName(current);
                if (parent == null)
                    break;

                if (!paths.Any(x => string.Equals(x.Path, parent, _pathComparer)))
                {
                    paths.Add((parent, null));
                }

                if (string.Equals(parent, root, _pathComparer))
                    break;

                current = parent;
            }
        }

        void AddNode((string Path, TagDataOperation? operation) node, int depth = 0)
        {
            var root = Path.GetPathRoot(node.Path);

            var name = string.Equals(node.Path, root, _pathComparer)
                ? root
                : Path.GetFileName(node.Path);

            var operation = operations.FirstOrDefault(x =>
                string.Equals(x.OriginalPath, node.Path, _pathComparer)
            );

            var indent = new string(' ', depth * 2);
            var prefix =
                node.operation is null ? "[bold]▸[/] "
                : operation is { HasChanges: true } ? "* "
                : "  ";
            var text = $"{indent}{prefix}{name.EscapeMarkup()}";
            text = text.Substring(0, Math.Min(text.Length, Console.WindowWidth))
                .PadRight(Console.WindowWidth);

            var line = new TreeLine(text, operation);

            treeLines.Add(line);

            var children = paths
                .Where(x => string.Equals(Path.GetDirectoryName(x.Path), node.Path, _pathComparer))
                .ToList();

            foreach (var child in children)
            {
                AddNode(child, depth + 1);
            }
        }

        var roots = paths
            .Where(x => string.Equals(x.Path, Path.GetPathRoot(x.Path), _pathComparer))
            .ToList();

        foreach (var root in roots)
        {
            AddNode(root);
        }

        return treeLines;
    }

    private record TreeLine(string Markup, TagDataOperation? Operation);
}
