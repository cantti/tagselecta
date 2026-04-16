# CLI commands

<!-- start:cli-help -->
### Help command

```
USAGE:
    tagselecta [OPTIONS] <COMMAND>

EXAMPLES:
    tagselecta edit song.mp3 --title 'Song 1' --artist 'Artist1;Artist 2' 
--key description --value test
    tagselecta edit song.mp3 --comment 'url=https://github.com'
    tagselecta edit song.mp3 --artist '{{ artist | regex.replace "^VA$" 
"Various Artists" "-i" }}'
    tagselecta discogs . --release 
https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots
    tagselecta find . --query "{{ title | string.downcase |  string.contains
'dub' }}"

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
    musicbrainz <path>       Update album from musicbrainz. You can pass        
                             musicbrainz release id (not master) or query to    
                             search                                             
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
    tagselecta edit song.mp3 --title 'Song 1' --artist 'Artist1;Artist 2' 
--key description --value test
    tagselecta edit song.mp3 --comment 'url=https://github.com'
    tagselecta edit song.mp3 --artist '{{ artist | regex.replace "^VA$" 
"Various Artists" "-i" }}'

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help            Prints help information                               
        --yes             Skip confirmation before writing changes to files     
        --album           Album name                                            
        --albumartist     One or more album artists. Multiple values can be     
                          provided using a ';' separator                        
        --artist          One or more artists. Multiple values can be provided  
                          using a ';' separator                                 
        --bpm             Beat per minutes                                      
        --comment         Comment or notes                                      
        --composer        Composer                                              
        --conductor       Conductor                                             
        --copyright       Copyright                                             
        --date            Release date                                          
        --discnumber      Disc number                                           
        --disctotal       Total number of discs                                 
        --genre           One or more genres. Multiple values can be provided   
                          using a ';' separator                                 
        --isrc            International standard recording code                 
        --publisher       Publisher                                             
        --title           Track title                                           
        --tracknumber     Track number                                          
        --tracktotal      Total number of tracks                                
    -k, --key             Extra field key key. Must be used together with       
                          --value                                               
    -v, --value                                                                 
        --clear           Clear all fields                                      
        --picture         Path or url to a picture. Use this option multiple    
                          times to include multiple images (e.g., -p path1 -p   
                          path2)                                                
    -P, --picturetype     Type of each picture provided. Specify multiple times 
                          to match the order of the pictures. This option is    
                          optional.                                             
                          Common values: FrontCover, BackCover, Artist, Other   
        --clearpicture    Clear all other pictures
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
                       DEFAULT                                                  
    -h, --help                                Prints help information           
        --yes                                 Skip confirmation before writing  
                                              changes to files                  
    -s, --separator    [",", ";", "feat."]    Separator. Can be used multiple   
                                              times
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
    tagselecta discogs . --release 
https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help       Prints help information                          
        --yes        Skip confirmation before writing changes to files
    -r, --release    Discogs release or master URL. Required
```

### Find command

```
DESCRIPTION:
Find files by metadata

USAGE:
    tagselecta find <path> [OPTIONS]

EXAMPLES:
    tagselecta find . --query "{{ title | string.downcase |  string.contains
'dub' }}"

ARGUMENTS:
    <path>     

OPTIONS:
    -h, --help     Prints help information
    -q, --query    Find query
```


<!-- end:cli-help -->
