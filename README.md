# TagSelecta

TagSelecta is a cross-platform, opinionated command-line tool for managing audio file metadata (tags).

https://github.com/user-attachments/assets/2b540004-5563-4b5c-ae78-f55f6508fda0

This tool is under active development and primarily built for personal use.
However, if you need additional features - for example, support for new tag types - feel free to open an [open an issue](https://github.com/cantti/audio-tag-helper/issues).
Adding new tags or functionality is straightforward and contributions are welcome.

The CLI is built using [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich command-line output and [TagLibSharp](https://github.com/mono/taglib-sharp) for tag manipulation.

## Features

- Works with both files and directories (recursively) as input
- Edit command to read and write tags
- And many other commands to work with audio metadata. See the full list below.

# Install

## Option 1. Download Release (Manual Install)

1. Go to the **[Releases page](https://github.com/cantti/tagselecta/releases)**
2. Download the latest archive for your system
3. Extract it
4. Move the binary into your preferred location (for example):

```sh
mv tagselecta "$HOME/.local/bin"
```

Ensure `"$HOME/.local/bin"` is in your `PATH`.

## Option 2. Install via Script (Automatic Install)

You can install the latest release automatically using the provided installer script:

```sh
wget -qO- https://raw.githubusercontent.com/cantti/tagselecta/main/install.sh | bash -s "$HOME/.local/bin"
```

This will:

* download the latest release,
* extract it,
* install it into the directory you provide (default recommended: `$HOME/.local/bin`).

Works from **bash**, **zsh**, **fish**, and other shells.

## Option 3. Download Script Manually

If you prefer to inspect the script before running it:

```sh
wget https://raw.githubusercontent.com/cantti/tagselecta/main/install.sh
chmod +x install.sh
./install.sh "$HOME/.local/bin"
```

## Usage

The program support multiple commands:

<!-- start:cli-help -->
### Help command

```
USAGE:
    tagselecta [OPTIONS] <COMMAND>

EXAMPLES:
    tagselecta edit song.mp3 -t 'Song 1' -a 'Artist1;Artist 2' -s 
description=test
    tagselecta edit song.mp3 -c 'url=https://github.com'
    tagselecta edit song.mp3 -a '{{ artist | regex.replace "^VA$" "Various 
Artists" "-i" }}'
    tagselecta discogs path-to-album -r 
https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots
    tagselecta discogs path-to-album -q King Tubby Dub From The Roots

OPTIONS:
    -h, --help       Prints help information   
    -v, --version    Prints version information

COMMANDS:
    ui <path>                Interactive UI (TUI)                               
    edit <path>              Edit (read and write) tags. Unrecognized options   
                             are saved as custom fields. Another way to edit    
                             custom fields is to use --custom option            
    extractpicture <path>    Extract pictures to files                          
    titlecase <path>         Convert all fields to title case                   
    split <path>             Split artists, album artists and composers         
    discogs <path>           Update album from discogs. You can pass discogs    
                             release id (not master) or query to search         
    autotrack <path>         Auto track                                         
    fixalbum <path>          Set album name, date and album artists to the same 
                             value to all files in the same directory           
    rename <path>            Rename files based on tag data                     
    find <path>              Find files by metadata
```

### Edit command

```
DESCRIPTION:
Edit (read and write) tags. Unrecognized options are saved as custom fields. 
Another way to edit custom fields is to use --custom option

USAGE:
    tagselecta edit <path> [OPTIONS]

EXAMPLES:
    tagselecta edit song.mp3 -t 'Song 1' -a 'Artist1;Artist 2' -s 
description=test
    tagselecta edit song.mp3 -c 'url=https://github.com'
    tagselecta edit song.mp3 -a '{{ artist | regex.replace "^VA$" "Various 
Artists" "-i" }}'

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help                Prints help information                           
    -l, --album               Album name                                        
    -A, --albumartist         One or more album artists. Multiple values can be 
                              provided using a ';' separator                    
    -a, --artist              One or more artists. Multiple values can be       
                              provided using a ';' separator                    
        --Bpm                 Beat per minutes                                  
        --catalognumber       Catalog number                                    
    -c, --comment             Comment or notes                                  
    -C, --composer            Composer                                          
        --conductor           Conductor                                         
        --copyright           Copyright                                         
    -y, --date                Release date                                      
    -d, --disc                Disc number                                       
    -D, --disctotal           Total number of discs                             
        --discogsreleaseid    Discogs release id                                
    -g, --genre               One or more genres. Multiple values can be        
                              provided using a ';' separator                    
        --isrc                International standard recording code             
        --label               Record label                                      
        --publisher           Publisher                                         
    -t, --title               Track title                                       
    -n, --track               Track number                                      
    -N, --tracktotal          Total number of tracks                            
    -s, --set                 Custom fields in key=value format. Use this option
                              multiple times to include multiple fields (e.g.,  
                              -c key1=value1 -c key2=value2)                    
        --clearcustom         Clear all other custom fields                     
    -p, --picture             Path to a picture file. Use this option multiple  
                              times to include multiple images (e.g., -p path1  
                              -p path2)                                         
        --picturetype         Type of each picture provided. Specify multiple   
                              times to match the order of the pictures. This    
                              option is optional.                               
                              Common values: FrontCover, BackCover, Artist,     
                              Other                                             
        --clearpicture        Clear all other pictures
```

### Extract Picture command

```
DESCRIPTION:
Extract pictures to files

USAGE:
    tagselecta extractpicture <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help        Prints help information                                   
    -t, --type        Types of pictures to extract. Multiple entries can be     
                      provided using a ';' separator.                           
                      Common types: FrontCover, BackCover, Artist, Other        
    -o, --output      Output file name                                          
        --override    Override files                                            
    -l, --limit       Limit number of files to be extracted
```

### Rename command

```
DESCRIPTION:
Rename files based on tag data

USAGE:
    tagselecta rename <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help        Prints help information                        
    -t, --template    Template. For example: {{ date }} - {{ album }}
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

### Title case command

```
DESCRIPTION:
Convert all fields to title case

USAGE:
    tagselecta titlecase <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help    Prints help information
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

### Fix Album command

```
DESCRIPTION:
Set album name, date and album artists to the same value to all files in the 
same directory

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
Update album from discogs. You can pass discogs release id (not master) or query
to search

USAGE:
    tagselecta discogs <path> [OPTIONS]

EXAMPLES:
    tagselecta discogs path-to-album -r 
https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots
    tagselecta discogs path-to-album -q King Tubby Dub From The Roots

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help       Prints help information                                    
    -r, --release                                                               
    -f, --fields     Fields to update from Discogs release. If not specified,   
                     all values will be updated
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

You can awlays get reference for formatting using `tagselecta helpformatting` or information below.

### Replace va with Various Artists

Assign Scriban expression to environment variable and use it to replace value of artist and albumartist. 

```sh
REPLACE='regex.replace "^va$" "Various Artists" "-i"' tagselecta edit . -a "{{ artist | $REPLACE }}" -A "{{ albumartist | $REPLACE }}"
```

### Clean tags

Common action to remove fields you do not need. The command below will remove label and catalognumber and all custom fields except url.

```sh
tagselecta edit ./song.mp3 --label '' --catalognumber '' --clearcustom -s 'url={{ custom.url }}'
```

Or just remove all custom tags:

```sh
tagselecta edit ./song.mp3 --clearcustom
```

### Extract picture to file and remove from tags

```sh
tagselecta extractpicture ./song.mp3 -o cover && tagselecta edit ./song.mp3 --clearpicture
```

Multiple pictures will be saved as cover, cover(1), cover(2), etc. Front covers come first.

## Formatting

<!-- start:formatting -->
```
Tagselecta uses the Scriban template engine when formatting fields and when 
renaming files or directories.

Example:
{{ year }} - {{ album }}

Useful links:
https://github.com/scriban/scriban/blob/master/doc/language.md
https://github.com/scriban/scriban/blob/master/doc/builtins.md#string-functions

Below is the list of available template fields:
path             Full file path.                         
filename         File name without extension.            
album            Album name.                             
albumartist      Album artists as a single string.       
albumartists     List of album artists.                  
artist           Artists as a single string.             
artists          List of artists.                        
bpm              Beats per minute.                       
catalognumber    Catalog number.                         
comment          User comment.                           
composer         Composers as a single string.           
composers        List of composers.                      
conductor        Conductor name.                         
copyright        Copyright text.                         
date             Original date value.                    
disc             Disc number.                            
disctotal        Total number of discs.                  
discogsreleaseid Discogs release ID.                     
genre            Genres as a single string.              
genres           List of genres.                         
isrc             ISRC code.                              
label            Record label.                           
publisher        Publisher.                              
title            Track title.                            
track            Track number.                           
tracktotal       Total number of tracks.                 
year             Year extracted from the Date field.     
custom           Custom fields. Usage example: custom.url
```
<!-- end:formatting -->

## Settings

Some global settings will be possible set via environment variables soon.

## Notes

- Designed for consistent and efficient tag management.  
- Works seamlessly across major platforms.
