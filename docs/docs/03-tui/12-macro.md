# Macros

Macro is one or more commands that can be executed using `:macro` command.

Macros are defined in the config file: `~/.config/tagselecta/config.toml`.

Example:

```toml
[macros]
reggae="edit genre=Reggae"
move="move t=\"../{{ year }} - {{ album }}/{{ discnumber ? pad(discnumber) + '.' : '' }}{{ pad(tracknumber) }}. {{ title }}.{{ ext }}\""
```

To call a macro use `:macro <name>` (`:m <name>`) command.

Macros are only inserted into the command line (commands are not executed immediately), you can edit them before executing.

You can chain multiple commands in a single macro using `&&`.

Example:

```toml
[macros]
cleanup="selectall && clear comment picture && write"
```

Macros can use tag-editing commands (`edit`, `move`, `clear`, etc.) and TUI commands.
Useful TUI commands for macros include:

- `selectall`
- `write` (`w`)
