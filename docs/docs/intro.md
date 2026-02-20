---
sidebar_position: 1
slug: /
---

# Get Started

![TagSelecta screenshot](../static/tagselecta.png)

TagSelecta is a cross-platform, opinionated command-line tool for managing audio file metadata (tags).

The tool supports two modes: interactive UI (TUI) and command-line interface (CLI).

To run the TUI, simply run `tagselecta ui <path>`.

To execute commands from the CLI, run `tagselecta <command> <path>`.

`Path` can be a single file or a directory (recursive).

Some commands available only in the CLI mode: `find` to find  files by metadata.

The CLI is built using [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich command-line output and [TagLibSharp](https://github.com/mono/taglib-sharp) for tag manipulation.
