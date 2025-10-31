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

<!-- CLI_HELP_START -->
```
[38;5;11mUSAGE:[0m
    tagselecta.dll [38;5;8m[OPTIONS][0m [38;5;14m<COMMAND>[0m

[38;5;11mOPTIONS:[0m
    -h, --help    Prints help information

[38;5;11mCOMMANDS:[0m
    [38;5;7mread[0m [38;5;7m<path>[0m          Read tags                                                                            
    [38;5;7mwrite[0m [38;5;7m<path>[0m         Write tags                                                                           
    [38;5;7mclean[0m [38;5;7m<path>[0m         Remove unsupported tags                                                              
    [38;5;7mfixalbum[0m [38;5;7m<path>[0m      Set album name and album artists to the same value to all files in the same directory
    [38;5;7mautotrack[0m [38;5;7m<path>[0m     Auto track                                                                           
    [38;5;7mrenamedir[0m [38;5;7m<path>[0m     Rename directories                                                                   
    [38;5;7mrenamefile[0m [38;5;7m<path>[0m    Rename files
```
<!-- CLI_HELP_END -->

## Notes

- Designed for consistent and efficient tag management.  
- Works seamlessly across major platforms.
