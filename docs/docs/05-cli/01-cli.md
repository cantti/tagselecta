# CLI Usage

CLI mode is useful for scripts and quick edits.

Basic syntax:

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

