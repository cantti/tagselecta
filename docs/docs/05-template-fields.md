# Templates

TagSelecta uses [Scriban](https://scriban.github.io/) templates to format text from file metadata. Templates are supported in `:edit` values, `:move` destinations, the TUI [`--filter` option](03-tui/01-get-started.md#filter-files-at-startup), and the CLI [`find` query](05-cli/02-find.md). In the TUI, `:help templates` shows a quick reference.

## Use fields

Put a field name inside `{{ }}` to insert its value. For example, copy the album artist to the artist field:

```text
:edit artist="{{ albumartist }}"
```

You can use standard and custom tag fields. Multiple values are joined with `; `. Quote a command value when it contains spaces.

## Apply functions

Use a pipe (`|`) to pass a value to a Scriban function:

```text
:edit genre="{{ genre | string.downcase }}"
```

This converts `Reggae` to `reggae`. See Scriban's [language reference](https://scriban.github.io/docs/language) and [built-in functions](https://scriban.github.io/docs/builtins) for more options.

## Rename or move files

Use metadata to build a destination path:

```text
:move template="../{{ year }} - {{ album }}/{{ pad(tracknumber) }}. {{ title }}.{{ ext }}"
```

For a 2020 album named `Album`, track `1` titled `Song`, and an MP3 file, the destination is `../2020 - Album/01. Song.mp3`, relative to the original file's directory. Run `:write` to apply the move.

## Extra fields and functions

In addition to tag fields, templates can use:

| Name | Value |
| --- | --- |
| `filename` | Original filename without its extension |
| `ext` | File extension without the dot |
| `path` | Original file path |
| `year` | Year from `date`, if it matches `yyyy`, `yyyy-MM-dd`, or `yyyy/MM/dd`; otherwise empty |
| `pad(value)` | Pad a value with leading zeros to at least two characters, for example `pad(tracknumber)` turns `1` into `01` |

`:edit` templates use metadata from when the file was loaded or last written. `:move` templates use the current metadata, including pending edits.
