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

### Help command

```
USAGE:
    tagselecta [OPTIONS] <COMMAND>

EXAMPLES:
    tagselecta write song.mp3 -t 'Song 1' -a 'Artist1;Artist 2'
    tagselecta write song.mp3 -c 'url=https://github.com'
    tagselecta write song.mp3 -t 'My Track' -l 'Best Album' -y 2024
    tagselecta write song.mp3 -g 'Rock;Heavy Metal;Punk'
    tagselecta write song.mp3 -a 'John Doe' -A 'Various Artists' --label 'Example Records'

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    read <path>          Read tags                                                                                  
    write <path>         Write tags                                                                                 
    split <path>         Split artists, album artists and composers                                                 
    autotrack <path>     Auto track                                                                                 
    renamedir <path>     Rename directories                                                                         
    renamefile <path>    Rename files                                                                               
    fixalbum <path>      Set album name, year and album artists to the same value to all files in the same directory
    discogs <path>       Update album from discogs. You can pass discogs release id (not master) or query to search 
    titlecase <path>     Convert all fields to title case                                                           
    find <path>          Find files by metadata
```

### Read command

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

### Write command

```
DESCRIPTION:
Write tags

USAGE:
    tagselecta write <path> [OPTIONS]

EXAMPLES:
    tagselecta write song.mp3 -t 'Song 1' -a 'Artist1;Artist 2'
    tagselecta write song.mp3 -c 'url=https://github.com'
    tagselecta write song.mp3 -t 'My Track' -l 'Best Album' -y 2024
    tagselecta write song.mp3 -g 'Rock;Heavy Metal;Punk'
    tagselecta write song.mp3 -a 'John Doe' -A 'Various Artists' --label 'Example Records'
    tagselecta write song.mp3 -n 5 -N 12 -d 1 -D 2
    tagselecta write song.mp3 --composers 'Composer 1;Composer 2;Composer 3'
    tagselecta write song.mp3 -c 'key1=value1;key2=Some Value;key3=Another Value'
    tagselecta write song.mp3 -a '{{ artist | regex.replace "^VA$" "Various Artists" "-i" }}'

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help             Prints help information                                                                                            
    -a, --artist           One or more artists. Multiple values can be provided using a ';' separator                                         
    -A, --albumartist      One or more album artists. Multiple values can be provided using a ';' separator                                   
    -t, --title            Track title                                                                                                        
    -l, --album            Album name                                                                                                         
    -y, --year             Release year                                                                                                       
    -g, --genre            One or more genres. Multiple values can be provided using a ';' separator                                          
    -n, --track            Track number                                                                                                       
    -N, --tracktotal       Total number of tracks                                                                                             
    -C, --comment          Comment or notes                                                                                                   
    -d, --disc             Disc number                                                                                                        
    -D, --disctotal        Total number of discs                                                                                              
        --composers        One or more composers. Multiple values can be provided using a ';' separator                                       
        --label            Record label                                                                                                       
        --catalognumber    Catalog number                                                                                                     
    -c, --custom           Custom tags in key=value format. Multiple entries can be provided using a ';' separator (e.g., key1=val1;key2=val2)
        --clear-custom     Clear all other custom tags, not specified using --custom or -c
```

### Split command

```
DESCRIPTION:
Split artists, album artists and composers

USAGE:
    tagselecta split <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help         Prints help information       
    -s, --separator    Default values are: , ; feat.
```

### Auto Track command

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

### Rename Directory command

```
DESCRIPTION:
Rename directories

USAGE:
    tagselecta renamedir <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help        Prints help information                       
    -t, --template    Template. For example: {{ year } - {{ album }}
```

### Rename File command

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
```

### Fix Album command

```
DESCRIPTION:
Set album name, year and album artists to the same value to all files in the same directory

USAGE:
    tagselecta fixalbum <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help    Prints help information
```

### Discogs command

```
DESCRIPTION:
Update album from discogs. You can pass discogs release id (not master) or query to search

USAGE:
    tagselecta discogs <path> [OPTIONS]

EXAMPLES:
    tagselecta discogs path-to-album -r https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots
    tagselecta discogs path-to-album -q King Tubby Dub From The Roots

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help       Prints help information                                                            
    -r, --release                                                                                       
    -f, --fields     Fields to update from Discogs release. If not specified, all values will be updated
```

### Find command

```
DESCRIPTION:
Find files by metadata

USAGE:
    tagselecta find <path> [OPTIONS]

EXAMPLES:
    tagselecta find . -q "title | string.downcase |  string.contains 'dub'"

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help     Prints help information
    -q, --query    Find query
```


<!-- end:cli-help -->

## More examples


Tagselecta uses the Scriban template engine when writing fields, allowing you to define powerful and flexible actions.
You can explore Scriban’s built-in functions here: [https://github.com/scriban/scriban/blob/master/doc/builtins.md](https://github.com/scriban/scriban/blob/master/doc/builtins.md)

It also integrates smoothly with shell functionality, enabling even more advanced workflows.


### Replace va with Various Artists

```sh
tagselecta write ./song.mp3 -a '{{ artist | regex.replace "^VA$" "Various Artists" "-i" }}' -A '{{ albumartist | regex.replace "^VA$" "Various Artists" "-i" }}'

```

### Clean tags

Common action to remove fields you do not need. The command below will remove label and catalognumber and all custom tags except url.

```sh
tagselecta write ./song.mp3 --label '' -catalognumber '' --clear-custom --custom 'url={{ custom.url }}'
```

Or just remove all custom tags:

```sh
tagselecta write ./song.mp3 --clear-custom
```


## Settings

Some global settings will be possible set via environment variables soon.

## Notes

- Designed for consistent and efficient tag management.  
- Works seamlessly across major platforms.
