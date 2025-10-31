# TagSelecta

TagSelecta is a cross-platform, opinionated command-line tool for managing audio file metadata (tags).  
Currently, only **MP3** and **FLAC** formats are supported.

This tool is under active development and primarily built for personal use.
However, if you need additional features - for example, support for new tag types - feel free to open an [open an issue](https://github.com/cantti/audio-tag-helper/issues).
Adding new tags or functionality is straightforward and contributions are welcome.

The CLI is built using [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich command-line output and [TagLibSharp](https://github.com/mono/taglib-sharp) for tag manipulation.

## Download

Download the latest release from the [Releases page](https://github.com/cantti/audio-tag-helper/releases).

## Features

- Works with both files and directories (recursively) as input
- Read command to read tags
- Write command to update tags
- Clean command to remove unsupported tags
- Fix album command to set album name and album artists to the same value to all files in the same directory.
- Autotrack command

## Usage

Each command provides its own help information.  
To display all available commands and options, run:

```bash
./tagselecta -h
```

```
USAGE:
    tagselecta.dll [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    read <path>          Read tags
    write <path>         Write tags
    clean <path>         Remove unsupported tags
    fixalbum <path>      Set album name and album artists to the same value to all files in the same directory
    autotrack <path>     Auto track
    renamedir <path>     Rename directories
    renamefile <path>    Rename files
```

Read command output:

```
./tagselecta read song.mp3
```

```
1 file found.
> (1/1) /home/kirill/code/songtagcli/TestData/01 SampleAlbum/01 Song 1.mp3
{
   "Artist": [
      "Test Artist"
   ],
   "AlbumArtist": [
      "Test Album Artist"
   ],
   "Album": "Test Album",
   "Title": "Song 1",
   "Genre": [
      "Rock",
      "Pop"
   ],
   "Year": 1990,
   "Track": 1,
   "TrackTotal": 3,
   "Disc": 1,
   "DiscTotal": 1,
   "Comment": "Test",
   "Label": "",
   "CatalogNumber": "",
   "Bpm": 0,
   "Pictures": [
   ]
}
Status: success!
Finished! Processed 1 files, 0 skipped, 0 failed.```
```

## Notes

- Designed for consistent and efficient tag management.  
- Works seamlessly across major platforms.
