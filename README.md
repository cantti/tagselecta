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

The program support multiple commands:

<!-- start:cli-help -->

**Help command**

```
USAGE:
    tagselecta [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    read <path>          Read tags                                              
    write <path>         Write tags                                             
    clean <path>         Remove unsupported tags                                
    fixalbum <path>      Set album name and album artists to the same value to  
                         all files in the same directory                        
    autotrack <path>     Auto track                                             
    renamedir <path>     Rename directories                                     
    renamefile <path>    Rename files
```

**Read command**

```
DESCRIPTION:
Read tags

USAGE:
    tagselecta read <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help    Prints help information
```

**Write command**

```
DESCRIPTION:
Write tags

USAGE:
    tagselecta write <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help           Prints help information
    -g, --genre                                 
    -a, --artist                                
    -A, --albumartist                           
    -t, --title                                 
        --subtitle                              
    -l, --album                                 
    -y, --year                                  
    -n, --track                                 
    -N, --tracktotal                            
    -c, --comment                               
    -d, --disc                                  
    -D, --disctotal                             
    -L, --label                                 
    -C, --catalogno                             
        --bpm                                   
        --description                           
        --composers                             
        --conductor                             
        --isrc                                  
        --lyrics                                
        --publisher                             
        --copyright
```

**RenameDir command**

```
DESCRIPTION:
Rename directories

USAGE:
    tagselecta renamedir <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help        Prints help information                        
    -t, --template    Template. For example: {{ year }} - {{ album }}
        --dry-run
```

**RenameFile command**

```
DESCRIPTION:
Rename files

USAGE:
    tagselecta renamefile <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help        Prints help information                        
    -t, --template    Template. For example: {{ year }} - {{ album }}
        --dry-run
```

**Clean command**

```
DESCRIPTION:
Remove unsupported tags

USAGE:
    tagselecta clean <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help    Prints help information
```

**FixAlbum command**

```
DESCRIPTION:
Set album name and album artists to the same value to all files in the same 
directory

USAGE:
    tagselecta fixalbum <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help    Prints help information
```

**AutoTrack command**

```
DESCRIPTION:
Auto track

USAGE:
    tagselecta autotrack <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help        Prints help information  
        --keepdisk    Remove Disc and DiscTotal
```


<!-- end:cli-help -->

## Notes

- Designed for consistent and efficient tag management.  
- Works seamlessly across major platforms.
