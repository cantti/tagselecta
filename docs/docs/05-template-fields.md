# Template fields

When using Scriban templates (for example in `:move t=`), you can access any field using `{{ fieldname }}` syntax. In the TUI, use `:help templates` for examples.

For example `{{ artist }}`.

Commands that support templates: `:edit`, `:move` and `find` (CLI only).

> Notes:
> - `year` is additional field for convenience parsed from `date` only if `Date` matches one of: `yyyy`, `yyyy-MM-dd`, `yyyy/MM/dd`.
> - `pad(...)` function zero-pads a field (commonly the `track` field), e.g. `pad(track)`.
