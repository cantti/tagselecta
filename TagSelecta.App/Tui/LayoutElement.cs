using Spectre.Console.Rendering;

namespace TagSelecta.App.Tui;

public record LayoutElement(IRenderable Content, int Size);
