# FAQ

Tips and common solutions.

## Macro to clean tags

Music files often include tags you don't need. Use a macro like the one below to remove everything except the listed fields.

```toml
[macros]
clean="clearexcept picture album artist albumartist date genre title tracknumber tracktotal discnumber disctotal label catalognumber"
```

## Run command on startup

You can run any TUI command automatically on startup using `startup_command` in config.

Example (select all files and run clean macro):

```toml
[general]
startup_command = "selectall && run clean"

[macros]
clean="clearexcept picture album artist albumartist date genre title tracknumber tracktotal discnumber disctotal label catalognumber"
```
