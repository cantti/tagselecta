using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public record LayoutElement(IRenderable Content, int Size);
