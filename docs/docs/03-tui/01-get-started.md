# Get started

Great way to get started is to use the interactive UI (TUI). Open directory with album (audio files) and run:

```sh
tagselecta
```

This opens the current directory. Use `tagselecta <path>` or `tagselecta ui <path>` to open a specific file or directory.
If a path has the same name as a command, prefix it with `./`, for example `tagselecta ./edit`.

>Important: Do not run it from the root of your music library, because it will scan all files in the directory!

The explorer shows your files, the metadata panel shows the focused file, and the status panel shows messages and shortcuts. The metadata count includes the path and picture entries.

Press `m` or run `:togglemetadata` to hide or show the metadata panel. Metadata is visible by default; hiding it gives the explorer the full available height. Press `e` to toggle the explorer independently.

## Navigation

Navigate through files using arrow keys or vim bindings (`jk`). Use `Shift+J` and `Shift+K` to scroll metadata. The metadata position resets when you switch files.
Use `tab` or `space` to select a file, `esc` to clear the selection and close help or pictures, and `q` to exit.

Use `:help` (or `:h`) to show command help, `:help edit` for the edit command and standard tag fields, `:help templates` for template examples, or `:help keymap` for shortcuts. The `h` key also opens general help.

## Edit tags

Commands are executed using command mode (`:`). All command have the following format: `:command <option>=<value>`.
Value can be in double quotes if it contains spaces. 
Boolean options can be specified as just the option name (e.g., `option` instead of `option=true`).
Some commands accept positional arguments instead, such as `:macro <name>`, `:run <name>`, `:help <topic>`, and `:open <path>`.

Try running `:edit genre=Reggae`. This will edit genre field for selected files.
Use `tab` for inserting autocomplete suggestions. Use `ctrl+space` to trigger completion.

Tag edits and moves are not applied to audio files until you *write* them using `:write` (`:w`). `:extractpicture` writes picture files immediately.


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

## Move files

Very useful command is `move` (`mv`) to rename and move files.

Example:
```sh
:move template="../{{ date }} - {{ album }}/{{ pad(track) }}. {{ title }}.{{ext}}"
```

## Aliases

Most command and option have aliases. For example, `:e` is an alias for `:edit`, `:mv t=` is an alias for `:move template=` and so on.

## Completion

Completion suggests command names, tag action options, macro names, and help topics. It does not complete option values or paths. Use `ctrl+space` to trigger (and cycle through) completion.
Use `tab` to select completion.
