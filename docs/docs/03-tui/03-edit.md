# Edit command

The basic and most common use case is to edit tags for selected files.

Example:

```
:edit artist="Artist 1" title="Track Name"
```


The `edit` action updates tag fields on the selected audio files.
Any option you pass will **overwrite** the existing value for that field (after template formatting, if supported by your config). Options you don’t pass are left unchanged.

Some fields accept multiple values (artists, genres, etc.). Provide multiple values by separating them with a semicolon:

- `artist="Artist 1; Artist 2"`
- `genre="House; Deep House"`

### Standard tag fields

- `album` (`l`): Album name. Example: `album="Test Album"`
- `albumartist` (`A`): One or more album artists. Example: `albumartist="Artist 1; Artist 2"`
- `artist` (`a`): One or more artists. Example: `artist="Artist 1; Artist 2"`
- `bpm`: Beats per minute. Example: `bpm=128`
- `catalognumber`: Catalog number. Example: `catalognumber="ABC-001"`
- `comment` (`c`): Comment or notes. Example: `comment="Ripped from vinyl"`
- `composer` (`C`): Composer. Example: `composer="A; B"`
- `conductor`: Conductor. Example: `conductor="John Doe"`
- `copyright`: Copyright. Example: `copyright="© 1999 Label"`
- `date` (`y`): Release date. Examples: `date=1999` or `date=1999-06-01`
- `disc` (`d`): Disc number. Example: `disc=1`
- `disctotal` (`D`): Total number of discs. Example: `disctotal=2`
- `genre` (`g`): One or more genres. Example: `genre="House; Techno"`
- `isrc`: International Standard Recording Code. Example: `isrc="GBXXX0100001"`
- `label`: Record label. Example: `label="Warp"`
- `publisher`: Publisher. Example: `publisher="Warp Records"`
- `title` (`t`): Track title. Example: `title="Track Name"`
- `track` (`n`): Track number. Example: `track=5`
- `tracktotal` (`N`): Total number of tracks. Example: `tracktotal=12`
 
### Extra fields

- `--key key1 --value value1` (`-k key1 -v value1`): Set a field by key. If the key matches a known built-in tag field, that field is updated; otherwise it becomes a extra field. Example: `key=url value=https://example.com`. Can be used multiple times.
- `clearextra`: Clear all extra fields.

> Tip: `--key`/`--value` is useful for scripting or for fields not exposed as dedicated options.

### Pictures (cover art)

- `picture` (`p`): Add one or more pictures from file paths or url. Example: `picture=./cover.jpg`
- `picturetype`: Type for picture. Example: `picturetype=FrontCover`
- `clearpicture`: Remove all existing pictures before adding new ones.

- If you provide multiple `--picturetype` values, they are matched by index to `--picture`.
- If you provide fewer types than pictures, the first provided type may be reused.
- If no type is provided (or parsing fails), the default type is `FrontCover`.
