# TagSelecta

TagSelecta is a cross-platform, opinionated command-line tool for managing audio file metadata (tags).

https://github.com/user-attachments/assets/650ab503-4d2a-4dbb-9739-d78f781e2a61

The tool supports two modes: interactive UI (TUI) and command-line interface (CLI).

To run the TUI, simply run `tagselecta ui <path>`.

To execute commands from the CLI, run `tagselecta <command> <path>`.

`Path` can be a single file or a directory (recursive).

Some commands available only in the CLI mode: `find` to find  files by metadata.

The CLI is built using [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich command-line output and [TagLibSharp](https://github.com/mono/taglib-sharp) for tag manipulation.

## Features

- CLI and TUI modes
- Recursive directory scanning
- Macros support
- Previw of changes before applying them
- `:edit` command to read and write tags
- `:move` command to move and rename files
- `:extractpicture` command to extract pictures to files
- `:titlecase` command to convert all fields to title case
- `:split` command to split artists, album artists and composers
- `:autotrack` command to automatically set track number and total tracks based on disc and disc total
- `:discogs` command to update album metadata from Discogs release

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

```
tagselecta ui .
```

Do not run it from the root of your music library, because it will scan all files in the directory!

Conseptually, the UI is divided into two parts: top panel with list of files and bottom panel with file details.

Navigate through files using arrow keys or vim bindings (`jk`).

Use `q` to exit.

Use `tab` or `space` to select file. Use `esc` to unselect.

Commands are executed using command mode (`:`).

Try running `:edit genre=Reggae`. This will edit genre field for selected files.

To write changes to files use `:write` (`:w`) command.

All command have the following format: `:command <arg>=<value>`. Value can be in double quotes if it contains spaces.

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

Very usefull command is `move` (`mv`) to rename and move files.

Example:
```sh
:move template="../{{ date }} - {{ album }}/{{ track00 }}. {{ title }}.{{ext}}"
```

Most comnmand and opionts have aliases. For example, `:e` is an alias for `:edit`, `:mv t=` is an alias for `:move template=` and so on.

Great way to learn more about commands and options is to run `--help` from cli for each command.

For example:

```
tagselecta edit --help
```

## `edit` command

The basic and most common use case is to edit tags for selected files.

The `edit` action updates tag fields on the selected audio files.
Any option you pass will **overwrite** the existing value for that field (after template formatting, if supported by your config). Options you don’t pass are left unchanged.

### Multi-value fields

Some fields accept **multiple values** (artists, genres, etc.). Provide multiple values by separating them with a semicolon:

- `artist="Artist 1; Artist 2"`
- `genre="House; Deep House"`

### Standard tag fields

| Field | Short | Description | Notes / Examples |
|---|---:|---|---|
| `album` | `l` | Album name. | `album="Test Album"` |
| `albumartist` | `A` | One or more album artists. | `albumartist="Artist 1; Artist 2"` |
| `artist` | `a` | One or more artists. | `artist="Artist 1; Artist 2"` |
| `bpm` |  | Beats per minute. | `bpm=128` |
| `catalognumber` |  | Catalog number. | `catalognumber="ABC-001"` |
| `comment` | `c` | Comment or notes. | `comment="Ripped from vinyl"` |
| `composer` | `C` | Composer. | `composer="A; B"` |
| `conductor` |  | Conductor. | `conductor="John Doe"` |
| `copyright` |  | Copyright. | `copyright="© 1999 Label"` |
| `date` | `y` | Release date. | `date=1999` or `date=1999-06-01` |
| `disc` | `d` | Disc number. | `disc=1` |
| `disctotal` | `D` | Total number of discs. | `disctotal=2` |
| `genre` | `g` | One or more genres. | `genre="House; Techno"` |
| `isrc` |  | International Standard Recording Code. | `isrc="GBXXX0100001"` |
| `label` |  | Record label. | `label="Warp"` |
| `publisher` |  | Publisher. | `publisher="Warp Records"` |
| `title` | `t` | Track title. | `title="Track Name"` |
| `track` | `n` | Track number. | `track=5` |
| `tracktotal` | `N` | Total number of tracks. | `tracktotal=12` |

### Custom fields

| Option | Short | Value | Description | Notes / Examples                                  |
|---|---:|---|---|---------------------------------------------------|
| `set` | `s` | `key=value` (repeatable) | Set a field by key. If the key matches a known built-in tag field, that field is updated; otherwise it becomes a custom field. | Use multiple times: `set="catalogNumber=ABC-001"` |
| `clearcustom` |  | flag | Clear **all** custom fields before applying any `set` values. | `clearcustom set=my_field=123`                    |

> Tip: `--set` is useful for scripting or for fields not exposed as dedicated options.

### Pictures (cover art)

| Option | Short | Value | Description | Notes / Examples |
|---|---:|---|---|---|
| `--picture` | `-p` | path (repeatable) | Add one or more pictures from file paths. | `--picture cover.jpg --picture back.jpg` |
| `--picturetype` |  | string (repeatable) | Type for each picture, matching the order of `--picture`. Optional. | `--picture cover.jpg --picturetype FrontCover` |
| `--clearpicture` |  | flag | Remove all existing pictures before adding new ones. | `--clearpicture --picture cover.jpg` |

**Picture type behavior:**
- If you provide multiple `--picturetype` values, they are matched by index to `--picture`.
- If you provide fewer types than pictures, the first provided type may be reused.
- If no type is provided (or parsing fails), the default type is `FrontCover`.

## `move` command

Moves/renames files using a template.

| Option | Short | Required | Description                                                                      |
|---|---:|:---:|----------------------------------------------------------------------------------|
| `--template` | `-t` | yes | Destination template (e.g. `../{{ year }} - {{ album }}/{{ filename }}.{{ext}}`) |
| `--keepemptydirs` | `-k` | no | Keep empty source directories after moving                                       |
| `--donotmoveother` | `-d` | no | Only move the audio files (don’t move other files in the folder)                 |

## `split` command

Split artists, album artists and composers. Default separators are `,`, `;`, `.feat`. 

Can be customized using `--separator` option.

## `titlecase` command

Converts all fields to title case.

## `discogs` command

Set album metadata from Discogs.com. Use `--url|-u` option to specify the release URL.

## `autotrack` command

Set track number automatically based on position in directory.

## Template fields

When using templates (for example in `:move t`), TagSelecta exposes a `TagDataForTemplate` object.  
All fields are **strings** unless stated otherwise. List fields are provided both as a joined string and as a list.

> Notes:
> - `Disc00` / `Track00` are **zero-padded** only when the original value is numeric.
> - `Year` is parsed from `Date` only if `Date` matches one of: `yyyy`, `yyyy-MM-dd`, `yyyy/MM/dd`.
> - `Custom` fields are accessed via dot notation (example: `custom.url`).

### File/path fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `path` | string | Full file path. | `/music/Artist/Album/01 - Title.flac` |
| `filename` | string | File name without extension. | `01 - Title` |
| `ext` | string | File extension (without the dot). | `flac` |

### Album fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `album` | string | Album name. | `Selected Ambient Works 85-92` |
| `albumartist` | string | Album artists as a single string. | `Aphex Twin` |
| `albumartists` | list<string> | List of album artists. | `["Aphex Twin"]` |

### Artist fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `artist` | string | Track artists as a single string. | `Artist A; Artist B` |
| `artists` | list<string> | List of track artists. | `["Artist A", "Artist B"]` |

### Track fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `title` | string | Track title. | `Xtal` |
| `track` | string | Track number (raw value from tags). | `1` |
| `track00` | string | Track number padded to 2 digits when numeric. | `01` |
| `tracktotal` | string | Total number of tracks. | `12` |

### Disc fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `disc` | string | Disc number (raw value from tags). | `1` |
| `disc00` | string | Disc number padded to 2 digits when numeric. | `01` |
| `disctotal` | string | Total number of discs. | `2` |

### Date fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `date` | string | Original date value as stored in tags. | `1993-03-01` |
| `year` | string | Year extracted from `date` (if parseable). | `1993` |

### Metadata fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `genre` | string | Genres as a single string. | `Ambient; Electronic` |
| `genres` | list<string> | List of genres. | `["Ambient", "Electronic"]` |
| `label` | string | Record label. | `Warp` |
| `publisher` | string | Publisher/organization. | `Warp Records` |
| `catalognumber` | string | Catalog number. | `WARPCD01` |
| `bpm` | string | Beats per minute. | `128` |
| `isrc` | string | ISRC code. | `GBXYZ1200001` |
| `comment` | string | User comment. | `Ripped from CD` |
| `composer` | string | Composers as a single string. | `Composer A` |
| `composers` | list<string> | List of composers. | `["Composer A"]` |
| `conductor` | string | Conductor name. | `John Doe` |
| `copyright` | string | Copyright text. | `© 1993 Label` |

### Custom fields

| Template field | Type | Description | Example |
|---|---|---|---|
| `custom` | map<string,string> | Custom tag fields (normalized keys). Access with dot syntax. | `{{ custom.url }}` |

## Macros

Macros are set of predefined command.

Macros are defined in the config file: `~/.config/tagselecta/config.toml`.

Each macro can have aliases and list of commands.

Example:

```toml
[macro.reggae]
aliases=["r"]
commands=['e g=Reggae']

[macro.dnb]
commands=['e g="Drum & Bass"']
```

To call a macro use `:macro <name>` (`:m <name>`) command.

## CLI Usage

Below is documentation generated from the CLI help.

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
    edit <path>              Edit (read and write) tags. Unrecognized options   
                             are saved as custom fields. Another way to edit    
                             custom fields is to use --custom option            
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
    -h, --help             Prints help information                              
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
    -s, --set              Custom fields in key=value format. Use this option   
                           multiple times to include multiple fields (e.g., -c  
                           key1=value1 -c key2=value2)                          
        --clearcustom      Clear all other custom fields                        
    -p, --picture          Path to a picture file. Use this option multiple     
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
    -t, --template          Template. For example: {{ date }} - {{ album }}.    
                            Required                                            
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
