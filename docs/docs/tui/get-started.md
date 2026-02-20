---
sidebar_position: 1
---

# Get started

Great way to get started is to use the interactive UI (TUI). Open directory with album (audio files) and run:

```sh
tagselecta ui .
```

>Important: Do not run it from the root of your music library, because it will scan all files in the directory!

Conceptually, the UI is divided into two parts: top panel with list of files and bottom panel with file details.

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

Most command and option have aliases. For example, `:e` is an alias for `:edit`, `:mv t=` is an alias for `:move template=` and so on.
