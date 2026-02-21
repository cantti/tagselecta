# Macros

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
