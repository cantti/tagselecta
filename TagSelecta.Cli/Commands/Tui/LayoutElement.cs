using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Commands.Tui;

public record LayoutElement(IRenderable Content, int Size);
