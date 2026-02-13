using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class TreeListWidget(
    IEnumerable<TagDataActionTarget> files,
    TagDataActionTarget? focusedFile,
    int windowSize
) : Renderable
{
    private readonly List<TagDataActionTarget> _files = files.ToList();

    private readonly StringComparison _pathComparer = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var treeLines = BuildTreeLines();

        var focusedIndex = treeLines.FindIndex(x => x.File == focusedFile);
        var windowStart = focusedIndex - windowSize / 2;

        var maxStart = Math.Max(0, treeLines.Count - windowSize);
        windowStart = Math.Clamp(windowStart, 0, maxStart);

        var linesToPrint = Math.Min(windowSize, treeLines.Count - windowStart);

        var items = new List<IRenderable>(linesToPrint);

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
            text = text.Substring(0, Math.Min(text.Length, maxWidth)).PadRight(maxWidth);

            var style = new Style(
                treeLine.File is not null && treeLine.File.HasChanges ? Color.Red : Color.Default,
                treeLine.File is not null && treeLine.File == focusedFile
                    ? Color.Gray
                    : Color.Default
            );

            items.Add(new Text(text, style));
        }

        IRenderable content = new Rows(
            new Text($"Files ({_files.Count}):", new Style(Color.Yellow)),
            new Rows(items)
        );

        return content.Render(options, maxWidth);
    }

    private List<TreeLine> BuildTreeLines()
    {
        var sc =
            _pathComparer == StringComparison.OrdinalIgnoreCase
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

        // map path to file for performance
        var fileByPath = new Dictionary<string, TagDataActionTarget>(sc);
        foreach (var f in _files)
        {
            fileByPath[f.BackupPath] = f;
        }

        var seen = new HashSet<string>(sc);
        var allPaths = new List<string>();

        foreach (var file in _files)
        {
            var current = file.BackupPath;
            while (true)
            {
                if (seen.Add(current))
                {
                    allPaths.Add(current);
                }

                var parent = Path.GetDirectoryName(current);
                if (parent is null)
                {
                    break;
                }

                // stop when reached root
                var root = Path.GetPathRoot(current);
                if (root is not null && string.Equals(parent, root, _pathComparer))
                {
                    if (seen.Add(parent))
                    {
                        allPaths.Add(parent);
                    }

                    break;
                }

                current = parent;
            }
        }

        // parent => children
        var childrenByParent = new Dictionary<string, List<string>>(sc);
        foreach (var path in allPaths)
        {
            var parent = Path.GetDirectoryName(path);
            if (parent is null)
            {
                continue;
            }

            if (!childrenByParent.TryGetValue(parent, out var list))
            {
                list = new List<string>();
                childrenByParent[parent] = list;
            }

            list.Add(path);
        }

        var roots = allPaths
            .Where(p => string.Equals(p, Path.GetPathRoot(p), _pathComparer))
            .Distinct(sc)
            .OrderBy(p => p, sc)
            .ToList();

        var treeLines = new List<TreeLine>(allPaths.Count);

        foreach (var root in roots)
        {
            AddNode(root, 0);
        }

        return treeLines;

        void AddNode(string path, int depth)
        {
            var root = Path.GetPathRoot(path);
            var name =
                root is not null && string.Equals(path, root, _pathComparer)
                    ? root
                    : Path.GetFileName(path);

            fileByPath.TryGetValue(path, out var file);

            treeLines.Add(new TreeLine(name, depth, file));

            if (!childrenByParent.TryGetValue(path, out var children))
            {
                return;
            }

            foreach (var child in children)
            {
                AddNode(child, depth + 1);
            }
        }
    }

    private record TreeLine(string Name, int Depth, TagDataActionTarget? File);
}
