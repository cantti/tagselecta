---
sidebar_position: 5
---

# Template fields

When using templates (for example in `:move t=`), TagSelecta exposes a `TagDataForTemplate` object.  
All fields are strings unless stated otherwise. List fields are provided both as a joined string and as a list.

> Notes:
> - `Year` is parsed from `Date` only if `Date` matches one of: `yyyy`, `yyyy-MM-dd`, `yyyy/MM/dd`.
> - `Extra` fields are accessed via dot notation (example: `extra.url`).
> - `pad(...)` function zero-pads a field (commonly the `track` field), e.g. `pad(track)`.

- `album`: Album name. Example: `Selected Ambient Works 85-92`
- `albumartist`: Album artists as a single string. Example: `Aphex Twin`
- `albumartists`: List of album artists. Example: `["Aphex Twin"]`
- `artist`: Track artists as a single string. Example: `Artist A; Artist B`
- `artists` (list): List of track artists. Example: `[ "Artist A", "Artist B" ]`
- `bpm`: string. Beats per minute. Example: `128`
- `catalognumber`: string. Catalog number. Example: `WARPCD01`
- `comment`: string. User comment. Example: `Ripped from CD`
- `composer`: string. Composers as a single string. Example: `Composer A`
- `composers`: list. List of composers. Example: `[ "Composer A", "Composer B" ]`
- `conductor`: string. Conductor name. Example: `John Doe`
- `copyright`: string. Copyright text. Example: `© 1993 Label`
- `date`: Original date value as stored in tags. Example: `1993-03-01`
- `disc`: Disc number (raw value from tags). Example: `1`
- `disctotal`: Total number of discs. Example: `2`
- `ext`: File extension (without the dot). Example: `flac`
- `filename`: File name without extension. Example: `01 - Title`
- `genre`: string. Genres as a single string. Example: `Ambient; Electronic`
- `genres`: list. List of genres. Example: `[ "Ambient", "Electronic" ]`
- `isrc`: string. ISRC code. Example: `GBXYZ1200001`
- `label`: string. Record label. Example: `Warp`
- `path`: Full file path. Example: `/music/Artist/Album/01 - Title.flac`
- `publisher`: string. Publisher/organization. Example: `Warp Records`
- `title`: Track title. Example: `Xtal`
- `track`: Track number (raw value from tags). Example: `1`
- `track00`: Track number padded to 2 digits when numeric. Example: `01`
- `tracktotal`: Total number of tracks. Example: `12`
- `year`: Year extracted from `date` (if parseable). Example: `1993`
- `extra`: Extra fields as a map. Usage: `{{ extra.url }}`
