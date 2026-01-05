using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Tui;

public class TreeFileListFactory
{
    private readonly List<TreeLine> _treeLines = [];

    private readonly StringComparison _pathComparer = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    public void BuildTreeLines(List<TagDataOperation> operations)
    {
        _treeLines.Clear();

        var paths = new List<(string Path, bool IsDir)>();

        foreach (var operation in operations)
        {
            paths.Add((operation.Path, false));

            var current = operation.Path;
            var root = Path.GetPathRoot(current);

            while (true)
            {
                var parent = Path.GetDirectoryName(current);
                if (parent == null)
                    break;

                if (!paths.Any(x => string.Equals(x.Path, parent, _pathComparer)))
                {
                    paths.Add((parent, true));
                }

                if (string.Equals(parent, root, _pathComparer))
                    break;

                current = parent;
            }
        }

        void AddNode((string Path, bool IsDir) node, int depth = 0)
        {
            var root = Path.GetPathRoot(node.Path);

            var name = string.Equals(node.Path, root, _pathComparer)
                ? root
                : Path.GetFileName(node.Path);

            var operation = operations.FirstOrDefault(x =>
                string.Equals(x.Path, node.Path, _pathComparer)
            );

            var indent = new string(' ', depth * 2);
            var prefix =
                node.IsDir ? "[bold]▸[/] "
                : operation != null && operation.HasChanges ? "* "
                : "  ";
            var text = $"{indent}{prefix}{name.EscapeMarkup()}";
            text = text.Substring(0, Math.Min(text.Length, Console.WindowWidth))
                .PadRight(Console.WindowWidth);

            var line = new TreeLine(text, node.Path);

            _treeLines.Add(line);

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
    }

    public IRenderable Render(
        List<TagDataOperation> operations,
        int selectedOperationIndex,
        int windowSize,
        bool filter
    )
    {
        if (operations.Count == 0)
        {
            return Text.Empty;
        }

        var selectedOperation = operations[selectedOperationIndex];

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var selectedIndexInTree = _treeLines.FindIndex(x => x.Path == selectedOperation.Path);
        var windowStart = selectedIndexInTree - (windowSize / 2);

        // clamp so we dont go before 0 or past the last possible full window start
        var maxStart = Math.Max(0, _treeLines.Count - windowSize);
        windowStart = Math.Clamp(windowStart, 0, maxStart);

        var linesToPrint = Math.Min(windowSize, _treeLines.Count - windowStart);

        var items = new List<IRenderable>();

        for (var i = 0; i < linesToPrint; i++)
        {
            var itemIndex = windowStart + i;
            var treeLine = _treeLines[itemIndex];
            items.Add(
                new Markup(
                    treeLine.Markup,
                    new Style(
                        null,
                        string.Equals(treeLine.Path, selectedOperation.Path, _pathComparer)
                            ? Color.Gray
                            : Color.Default
                    )
                )
            );
        }

        return new Rows(
            new Text(
                $"Files ({operations.Count}{(filter ? ", filtered" : "")}):",
                new Style(Color.Yellow)
            ),
            new Rows(items)
        );
    }
}
