# Template fields

When using templates (for example in `:move t=`), TagSelecta exposes a `TagDataForTemplate` object.

To access any fild just use `{{ fieldname }}`. For example `{{ artist }}`.

All fields are strings unless stated otherwise.

> Notes:
> - `year` is additional field for convenience parsed from `date` only if `Date` matches one of: `yyyy`, `yyyy-MM-dd`, `yyyy/MM/dd`.
> - `Extra` fields are accessed via dot notation (example: `extra.url`).
> - `pad(...)` function zero-pads a field (commonly the `track` field), e.g. `pad(track)`.
