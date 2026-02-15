# TagSelecta

<img width="1820" height="947" alt="TagSelecta TUI" src="https://github.com/user-attachments/assets/b982bc08-dcbf-45c3-a86e-a3600f21cded" />


TagSelecta is a cross-platform, opinionated command-line tool for managing audio file metadata (tags).

The tool supports two modes: interactive UI (TUI) and command-line interface (CLI).

To run the TUI, simply run `tagselecta ui <path>`.

To execute commands from the CLI, run `tagselecta <command> <path>`.

`Path` can be a single file or a directory (recursive).

Some commands available only in the CLI mode: `find` to find  files by metadata.

The CLI is built using [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich command-line output and [TagLibSharp](https://github.com/mono/taglib-sharp) for tag manipulation.

## Features

- CLI and TUI modes
- `:edit` command to read and write tags
- `:move` command to move and rename files
- `:extractpicture` command to extract pictures to files
- `:titlecase` command to convert all fields to title case
- `:split` command to split artists, album artists and composers
- `:autotrack` command to automatically set track number and total tracks based on disc and disc total
- `:discogs` command to update album metadata from Discogs release
- `find` command to find files by metadata (CLI only)
- Recursive directory scanning
- Macros support
- Previw of changes before applying them
- Tree view of files

## TUI vs CLI

The program is primarily designed for use in TUI mode. However, the CLI mode is particularly useful for scripting and automating tasks.

## Install

### Option 1. Download Release (Manual Install)

1. Go to the **[Releases page](https://github.com/cantti/tagselecta/releases)**
2. Download the latest archive for your system
3. Extract it
4. Move the binary into your preferred location (for example):

```sh
mv tagselecta "$HOME/.local/bin"
```

Ensure `"$HOME/.local/bin"` is in your `PATH`.

### Option 2. Install via Script (Automatic Install)

You can install the latest release automatically using the provided installer script (review the script before running it):

```sh
wget -qO- https://raw.githubusercontent.com/cantti/tagselecta/main/install.sh | bash -s "$HOME/.local/bin"
```

This will:

* download the latest release,
* extract it,
* install it into the directory you provide (default recommended: `$HOME/.local/bin`).

Works from **bash**, **zsh**, **fish**, and other shells.

## Getting started

Great way to get started is to use the interactive UI (TUI). Open directory with album (audio files) and run:

```sh
tagselecta ui .
```

>Important: Do not run it from the root of your music library, because it will scan all files in the directory!

Conseptually, the UI is divided into two parts: top panel with list of files and bottom panel with file details.

**Navigation**

Navigate through files using arrow keys or vim bindings (`jk`). Use `q` to exit. 
Use `tab` or `space` to select file. Use `esc` to unselect.

**Edit tags**

Commands are executed using command mode (`:`). All command have the following format: `:command <option>=<value>`. Value can be in double quotes if it contains spaces.
Exception is `:macro` (`:m`) command which has just one argument: `:macro <macro_name>`.


Try running `:edit genre=Reggae`. This will edit genre field for selected files.

No changes are applied until you *write* them. To write changes to files use `:write` (`:w`) command.


Update multiple fields at once: `:edit genre=Reggae albumartist="King Tubby"`.

Format field values using Scriban template engine:

```sh
# Lowercase genre
:edit genre="{{ genre | string.downcase }}"

# Set artist from albumartist
:edit artist="{{ albumartist  }}"

# Set artist from albumartist
:edit artist="{{ albumartist  }}"

# Set title from filename
:edit title="{{ filename }}"
```

Other commands are implemented using the same format.

**Move files**

Very useful command is `move` (`mv`) to rename and move files.

Example:
```sh
:move template="../{{ date }} - {{ album }}/{{ pad(track) }}. {{ title }}.{{ext}}"
```

**Notes**

Most comnmand and option have aliases. For example, `:e` is an alias for `:edit`, `:mv t=` is an alias for `:move template=` and so on.

Great way to learn more about commands and options is to run `--help` from cli for each command.

For example:

```
tagselecta edit --help
```

Below is a detaled documentation for each command.

## `edit` command

The basic and most common use case is to edit tags for selected files.

The `edit` action updates tag fields on the selected audio files.
Any option you pass will **overwrite** the existing value for that field (after template formatting, if supported by your config). Options you don’t pass are left unchanged.

Some fields accept **multiple values** (artists, genres, etc.). Provide multiple values by separating them with a semicolon:

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
- `clearextra` (`e`): Clear **all** extra fields.

> Tip: `--key`/`--value` is useful for scripting or for fields not exposed as dedicated options.

### Pictures (cover art)

- `picture` (`p`): Add one or more pictures from file paths. Example: `picture=./cover.jpg`
- `picturetype`: Type for picture. Example: `picturetype=FrontCover`
- `clearpicture`: Remove all existing pictures before adding new ones.

- If you provide multiple `--picturetype` values, they are matched by index to `--picture`.
- If you provide fewer types than pictures, the first provided type may be reused.
- If no type is provided (or parsing fails), the default type is `FrontCover`.

## `move` command

Moves/renames files using a template.

- `template` (`t`): Destination template (e.g. `../{{ year }} - {{ album }}/{{ filename }}.{{ext}}`)
- `keepemptydirs` (`k`):Keep empty source directories after moving
- `donotmoveother` (`d`): Only move the audio files (don’t move other files in the folder)
## `split` command

Split artists, album artists and composers. Default separators are `,`, `;`, `.feat`. 

Can be customized using `--separator` option.

## `autotrack` command

Set track number automatically based on position in directory.

## `titlecase` command

Converts values of all fields to title case.

## `discogs` command

Set album metadata from Discogs.com. Use `--url|-u` option to specify the release URL.


## Template fields

When using templates (for example in `:move t=`), TagSelecta exposes a `TagDataForTemplate` object.  
All fields are **strings** unless stated otherwise. List fields are provided both as a joined string and as a list.

> Notes:
> - `Disc00` / `Track00` are **zero-padded** only when the original value is numeric.
> - `Year` is parsed from `Date` only if `Date` matches one of: `yyyy`, `yyyy-MM-dd`, `yyyy/MM/dd`.
> - `Extra` fields are accessed via dot notation (example: `extra.url`).

- `album`: Album name. Example: `Selected Ambient Works 85-92`
- `albumartist`: Album artists as a single string. Example: `Aphex Twin`
- `albumartists`: List of album artists. Example: `["Aphex Twin"]`
- `artist` (string): Track artists as a single string. Example: `Artist A; Artist B`
- `artists` (list<string>): List of track artists. Example: `["Artist A", "Artist B"]`
- `bpm`: string. Beats per minute. Example: `128`
- `catalognumber`: string. Catalog number. Example: `WARPCD01`
- `comment`: string. User comment. Example: `Ripped from CD`
- `composer`: string. Composers as a single string. Example: `Composer A`
- `composers`: list<string>. List of composers. Example: `["Composer A"]`
- `conductor`: string. Conductor name. Example: `John Doe`
- `copyright`: string. Copyright text. Example: `© 1993 Label`
- `date`: Original date value as stored in tags. Example: `1993-03-01`
- `disc`: Disc number (raw value from tags). Example: `1`
- `disc00`: Disc number padded to 2 digits when numeric. Example: `01`
- `disctotal`: Total number of discs. Example: `2`
- `ext`: File extension (without the dot). Example: `flac`
- `filename`: File name without extension. Example: `01 - Title`
- `genre`: string. Genres as a single string. Example: `Ambient; Electronic`
- `genres`: list<string>. List of genres. Example: `["Ambient", "Electronic"]`
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

## Macros

Macro is one or more commands that can be executed using `:macro` command.

Macros are defined in the config file: `~/.config/tagselecta/config.toml`.

Example:

```toml
[macros]
reggae="edit genre=Reggae"
clean="edit clearextra comment=\"\" && autotrack"
```

To call a macro use `:macro <name>` (`:m <name>`) command.

If a macro contains multiple commands, they are executed sequentially. If a macro has only one command, its text is
inserted into the command prompt, allowing you to modify it.

> Macro support is experimental.

## CLI Usage

Below is documentation generated from the CLI help.

The program support multiple commands:

<!-- start:cli-help -->
### Help command

```
USAGE:
    tagselecta [OPTIONS] <COMMAND>

EXAMPLES:
    tagselecta edit song.mp3 -t 'Song 1' -a 'Artist1;Artist 2' -k 
description -v test
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
    edit <path>              Edit tags (read/write). To edit extra fields, use  
                             the --key key1 --value value1 options              
    extractpicture <path>    Extract pictures to files                          
    titlecase <path>         Convert all fields to title case                   
    split <path>             Split artists, album artists and composers         
    discogs <path>           Update album from discogs. You can pass discogs    
                             release id (not master) or query to search         
    autotrack <path>         Auto track                                         
    move <path>              Move (rename) files to another directory           
    find <path>              Find files by metadata                             
    ui <path>                Interactive UI (TUI)
```

### Edit command

```
DESCRIPTION:
Edit tags (read/write). To edit extra fields, use the --key key1 --value value1 
options

USAGE:
    tagselecta edit <path> [OPTIONS]

EXAMPLES:
    tagselecta edit song.mp3 -t 'Song 1' -a 'Artist1;Artist 2' -k 
description -v test
    tagselecta edit song.mp3 -c 'url=https://github.com'
    tagselecta edit song.mp3 -a '{{ artist | regex.replace "^VA$" "Various 
Artists" "-i" }}'

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help             Prints help information                              
        --yes              Skip confirmation before writing changes to files    
    -l, --album            Album name                                           
    -A, --albumartist      One or more album artists. Multiple values can be    
                           provided using a ';' separator                       
    -a, --artist           One or more artists. Multiple values can be provided 
                           using a ';' separator                                
        --Bpm              Beat per minutes                                     
        --catalognumber    Catalog number                                       
    -c, --comment          Comment or notes                                     
    -C, --composer         Composer                                             
        --conductor        Conductor                                            
        --copyright        Copyright                                            
    -y, --date             Release date                                         
    -d, --disc             Disc number                                          
    -D, --disctotal        Total number of discs                                
    -g, --genre            One or more genres. Multiple values can be provided  
                           using a ';' separator                                
        --isrc             International standard recording code                
        --label            Record label                                         
        --publisher        Publisher                                            
    -t, --title            Track title                                          
    -n, --track            Track number                                         
    -N, --tracktotal       Total number of tracks                               
    -k, --key              Extra field key key. Must be used together with      
                           --value                                              
    -v, --value                                                                 
        --clearextra       Clear all other extra fields                         
    -p, --picture          Path or url to a picture. Use this option multiple   
                           times to include multiple images (e.g., -p path1 -p  
                           path2)                                               
        --picturetype      Type of each picture provided. Specify multiple times
                           to match the order of the pictures. This option is   
                           optional.                                            
                           Common values: FrontCover, BackCover, Artist, Other  
        --clearpicture     Clear all other pictures
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
        --yes         Skip confirmation before writing changes to files         
    -t, --type        Types of pictures to extract. Multiple entries can be     
                      provided using a ';' separator.                           
                      Common types: FrontCover, BackCover, Artist, Other        
    -o, --output      Output file name                                          
        --override    Override files                                            
    -l, --limit       Limit number of files to be extracted
```

### Move command

```
DESCRIPTION:
Move (rename) files to another directory

USAGE:
    tagselecta move <path> [OPTIONS]

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help              Prints help information                             
        --yes               Skip confirmation before writing changes to files   
    -t, --template          Template. For example: {{ track }} - {{ title }}.{{ 
                            ext }}. Required                                    
    -k, --keepemptydirs     Keep empty directories                              
    -d, --donotmoveother    Do not move other files
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
        --yes          Skip confirmation before writing changes to files
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
        --yes     Skip confirmation before writing changes to files
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
        --yes         Skip confirmation before writing changes to files
        --keepdisk    Remove Disc and DiscTotal
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
    -h, --help      Prints help information                                     
        --yes       Skip confirmation before writing changes to files           
    -u, --url       Discogs release URL. Required                               
    -f, --fields    Fields to update from Discogs release. If not specified, all
                    values will be updated
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

## Settings

Some global settings will be possible set via environment variables soon.

## Notes

- Designed for consistent and efficient tag management.  
- Works seamlessly across major platforms.
