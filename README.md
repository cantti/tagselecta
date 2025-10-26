# Audio Tag Helper

Audio Tag Helper is a cross-platform, opinionated command-line tool for managing audio file metadata (tags).  
Currently, only **MP3** and **FLAC** formats are supported.

This tool is under active development and primarily built for personal use.
However, if you need additional features - for example, support for new tag types - feel free to open an [open an issue](https://github.com/cantti/audio-tag-helper/issues).
Adding new tags or functionality is straightforward and contributions are welcome.

The CLI is built using [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich command-line output and [TagLibSharp](https://github.com/mono/taglib-sharp) for tag manipulation.

## Download

Download the latest release from the [Releases page](https://github.com/cantti/audio-tag-helper/releases).

## Usage

Each command provides its own help information.  
To display all available commands and options, run:

```bash
./songtagcli -h
```

### Example Output

Help output:

```bash
./songtagcli -h
```

```
USAGE:
    songtagcli [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    read <path>         Read tags
    write <path>        Write tags
    clean <path>        Remove unsupported tags
    fixalbum <path>     Set album name and album artist to the same value for all files in the directory
    autotrack <path>    Automatically assign track numbers to files
```

Read command output:

```
./songtagcli read song.mp3
```

```
1 file found.
> (1/1) /home/kirill/code/audio-tag-cli/tmp/01 SampleAlbum/01 Song 1.mp3
╭────────────────┬─────────────╮
│ Field          │ Value       │
├────────────────┼─────────────┤
│ Album Artist   │ Test Artist │
│ Artist         │ Test Artist │
│ Album          │ Test Album  │
│ Title          │ Song 1      │
│ Genre          │ Rock        │
│                │ Pop         │
│ Year           │ 1990        │
│ Track          │ 1           │
│ Track Total    │ 3           │
│ Disc           │ 1           │
│ Disc Total     │ 1           │
│ Label          │ —           │
│ Catalog Number │ —           │
│ Comments       │ Test        │
│ Pictures       │ —           │
╰────────────────┴─────────────╯
Finished! Processed 1 files, 0 failed.
```

## Notes

- Designed for consistent and efficient tag management.  
- Works seamlessly across major platforms.
