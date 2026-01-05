using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Tui;

public record LayoutElement(IRenderable Content, int Size);
