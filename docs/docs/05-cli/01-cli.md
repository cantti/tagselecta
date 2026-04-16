# CLI Usage

CLI mode is useful for scripts and quick edits.

Basic syntax:

```
USAGE:
    tagselecta.dll [OPTIONS] <COMMAND>

EXAMPLES:
    tagselecta.dll edit song.mp3 --title 'Song 1' --artist 'Artist1;Artist 2' --key description --value test
    tagselecta.dll edit song.mp3 --comment 'url=https://github.com'
    tagselecta.dll edit song.mp3 --artist '{{ artist | regex.replace "^VA$" "Various Artists" "-i" }}'
    tagselecta.dll discogs . --release https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots
    tagselecta.dll find . --query "{{ title | string.downcase |  string.contains 'dub' }}"

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    edit <path>              Edit tags (read/write). To edit extra fields, use the --key key1 --value value1 options
    extractpicture <path>    Extract pictures to files
    titlecase <path>         Convert all fields to title case
    split <path>             Split artists, album artists and composers
    discogs <path>           Update album from discogs. You can pass discogs release id (not master) or query to search
    autotrack <path>         Auto track
    move <path>              Move (rename) files to another directory
    find <path>              Find files by metadata
    musicbrainz <path>       Update album from musicbrainz. You can pass musicbrainz release id (not master) or query to search
    ui <path>                Interactive UI (TUI)
```
