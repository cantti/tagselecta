using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Commands.TagDataCommandShared.InteractiveWrite;

public record LayoutElement(IRenderable Content, int Size);
