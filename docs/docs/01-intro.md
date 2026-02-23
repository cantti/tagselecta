---
slug: /
---

# Intro

![TagSelecta screenshot](../static/tagselecta.png)

TagSelecta is a cross-platform, opinionated command-line tool for managing audio file metadata (tags).

The tool supports two modes: interactive UI (TUI) and command-line interface (CLI).

To run the TUI, simply run `tagselecta ui <path>`.

To execute commands from the CLI, run `tagselecta <command> <path>`.

`Path` can be a single file or a directory (recursive).

Some commands available only in the CLI mode: `find` to find  files by metadata.

The CLI is built using [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich command-line output and [TagLibSharp](https://github.com/mono/taglib-sharp) for tag manipulation.

## TUI vs CLI

The program is primarily designed for use in TUI mode. However, the CLI mode is particularly useful for scripting and automating tasks.

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
- Preview of changes before applying them
- Tree view of files
- Command history and autocompletion
