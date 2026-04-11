# Edit command

The basic and most common use case is to edit tags for selected files.

Example:

```
:edit artist="Artist 1" title="Track Name"
```

The `edit` action updates tag fields on the selected audio files.
Any option you pass will **overwrite** the existing value for that field (after template formatting, if supported by
your config). Options you don’t pass are left unchanged.

Some fields accept multiple values (albumartist, artist, composer, genre). Provide multiple values by separating them with a semicolon:

- `artist="Artist 1; Artist 2"`
- `genre="House; Deep House"`

### Standard tag fields

Standard tag fields have autocomplete suggestions in UI and stored in the standard text frames.

- `album`: Album name. Example: `album="Test Album"`
- `albumartist`: One or more album artists. Example: `albumartist="Artist 1; Artist 2"`
- `artist`: One or more artists. Example: `artist="Artist 1; Artist 2"`
- `bpm`: Beats per minute. Example: `bpm=128`
- `comment`: Comment or notes. Example: `comment="Ripped from vinyl"`
- `composer`: Composer. Example: `composer="A; B"`
- `conductor`: Conductor. Example: `conductor="John Doe"`
- `copyright`: Copyright. Example: `copyright="© 1999 Label"`
- `date`: Release date. Examples: `date=1999` or `date=1999-06-01`
- `discnumber`: Disc number. Example: `discnumber=1`
- `disctotal`: Total number of discs. Example: `disctotal=2`
- `genre`: One or more genres. Example: `genre="House; Techno"`
- `isrc`: International Standard Recording Code. Example: `isrc="GBXXX0100001"`
- `publisher`: Publisher. Example: `publisher="Warp Records"`
- `title`: Track title. Example: `title="Track Name"`
- `tracknumber`: Track number. Example: `tracknumber=5`
- `tracktotal`: Total number of tracks. Example: `tracktotal=12`

### Other fields

Other fields can be set similar to standard fields. Example: `label="Recorded at Home"`, `music_brainz_recording_id=1234567890abcdef`.

### Clear fields

To clear a field, assign an empty value. Example: `artist=""` or just `artist=`.

To remove all fields, use the `clear` option.

### Pictures (cover art)

- `picture`: Add one or more pictures from file paths or url. Example: `picture=./cover.jpg`
- `picturetype`: Type for picture. Example: `picturetype=FrontCover`
- `clearpicture`: Remove all existing pictures before adding new ones.

- If you provide multiple `--picturetype` values, they are matched by index to `--picture`.
- If you provide fewer types than pictures, the first provided type may be reused.
- If no type is provided (or parsing fails), the default type is `FrontCover`.

### Field mapping

TagSelecta uses normalized field names (lowercase), then maps them to format-specific tag fields.

| TagSelecta field | ID3v2 (MP3)                                               | FLAC/OGG (Xiph Comment) |
|------------------|-----------------------------------------------------------|-------------------------|
| `album`          | `TALB`                                                    | `album`                 |
| `albumartist`    | `TPE2`                                                    | `albumartist`           |
| `artist`         | `TPE1`                                                    | `artist`                |
| `bpm`            | `TBPM`                                                    | `bpm`                   |
| `comment`        | `COMM`                                                    | `comment`               |
| `composer`       | `TCOM`                                                    | `composer`              |
| `conductor`      | `TPE3`                                                    | `conductor`             |
| `copyright`      | `TCOP`                                                    | `copyright`             |
| `date`           | `TDRC`                                                    | `date`                  |
| `discnumber`     | `TPOS` (value part)                                       | `discnumber`            |
| `disctotal`      | `TPOS` (total part)                                       | `disctotal`             |
| `genre`          | `TCON`                                                    | `genre`                 |
| `isrc`           | `TSRC`                                                    | `isrc`                  |
| `publisher`      | `TPUB`                                                    | `organization`          |
| `title`          | `TIT2`                                                    | `title`                 |
| `tracknumber`    | `TRCK` (value part)                                       | `tracknumber`           |
| `tracktotal`     | `TRCK` (total part)                                       | `tracktotal`            |

Notes:

- Multi-value fields (like `artist`, `albumartist`, `composer`, `genre`) are stored as multiple values.
- Unknown/non-standard fields are stored as:
    - ID3v2: `TXXX` (description = field name)
    - FLAC/OGG: Xiph comment field with the same key
