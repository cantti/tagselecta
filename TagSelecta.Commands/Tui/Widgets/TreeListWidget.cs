using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class TreeListWidget(
    IEnumerable<TagDataActionTarget> files,
    TagDataActionTarget? focusedFile,
    int windowSize
) : Renderable
{
    private static List<TreeLine>? _cachedTreeLines;

    private readonly List<TagDataActionTarget> _files = files.ToList();

    private readonly StringComparison _pathComparer = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    // todo find better approach
    private List<TreeLine> GetTreeLines()
    {
        if (
            _cachedTreeLines is null
            || _cachedTreeLines.Count(x => x.File is not null) != _files.Count
        )
        {
            _cachedTreeLines = BuildTreeLines();
        }

        return _cachedTreeLines;
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var treeLines = GetTreeLines();

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var focusedIndex = treeLines.FindIndex(x => x.File == focusedFile);
        var windowStart = focusedIndex - windowSize / 2;

        // clamp so we dont go before 0 or past the last possible full window start
        var maxStart = Math.Max(0, treeLines.Count - windowSize);
        windowStart = Math.Clamp(windowStart, 0, maxStart);

        var linesToPrint = Math.Min(windowSize, treeLines.Count - windowStart);

        var items = new List<IRenderable>();

        for (var i = 0; i < linesToPrint; i++)
        {
            var itemIndex = windowStart + i;
            var treeLine = treeLines[itemIndex];
            var selectedMarker = treeLine.File is not null
                ? treeLine.File.IsSelected
                    ? "[x]"
                    : "[ ]"
                : "   ";
            var indent = new string(' ', treeLine.Depth * 2);
            var prefix = treeLine.File is null ? "▸ " : "  ";
            var text = $"{selectedMarker}{indent}{prefix}{treeLine.Name}";
            text = text.Substring(0, Math.Min(text.Length, Console.WindowWidth))
                .PadRight(Console.WindowWidth);

            var style = new Style(
                treeLine.File is not null && treeLine.File.HasChanges ? Color.Red : Color.Default,
                treeLine.File is not null && treeLine.File == focusedFile
                    ? Color.Gray
                    : Color.Default
            );
            items.Add(new Text(text, style));
        }

        IRenderable content = new Rows(
            new Text($"Files ({_files.Count()}):", new Style(Color.Yellow)),
            new Rows(items)
        );

        return content.Render(options, maxWidth);
    }

    private List<TreeLine> BuildTreeLines()
    {
        var treeLines = new List<TreeLine>();

        var paths = new List<(string Path, TagDataActionTarget? file)>();

        foreach (var file in _files)
        {
            paths.Add((file.BackupPath, file));

            var current = file.BackupPath;
            var root = Path.GetPathRoot(current);

            while (true)
            {
                var parent = Path.GetDirectoryName(current);
                if (parent == null)
                {
                    break;
                }

                if (!paths.Any(x => string.Equals(x.Path, parent, _pathComparer)))
                {
                    paths.Add((parent, null));
                }

                if (string.Equals(parent, root, _pathComparer))
                {
                    break;
                }

                current = parent;
            }
        }

        var roots = paths
            .Where(x => string.Equals(x.Path, Path.GetPathRoot(x.Path), _pathComparer))
            .ToList();

        foreach (var root in roots)
            AddNode(root);

        return treeLines;

        void AddNode((string Path, TagDataActionTarget? file) node, int depth = 0)
        {
            var root = Path.GetPathRoot(node.Path);

            var name = string.Equals(node.Path, root, _pathComparer)
                ? root
                : Path.GetFileName(node.Path);

            var file = _files.FirstOrDefault(x =>
                string.Equals(x.BackupPath, node.Path, _pathComparer)
            );

            var line = new TreeLine(name!, depth, file);

            treeLines.Add(line);

            var children = paths
                .Where(x => string.Equals(Path.GetDirectoryName(x.Path), node.Path, _pathComparer))
                .ToList();

            foreach (var child in children)
                AddNode(child, depth + 1);
        }
    }

    private record TreeLine(string Name, int Depth, TagDataActionTarget? File);
}
