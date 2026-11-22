# Settings

On Unix, the config file is at:

```
~/.config/tagselecta/config.toml
```

If `XDG_CONFIG_HOME` is set, it is at `$XDG_CONFIG_HOME/tagselecta/config.toml` instead. On Windows, it is at `%APPDATA%\tagselecta\config.toml`.

Below is an example of the config file with comments.

```toml
[general]
# debug mode
debug = false

# file list ratio in ui
file_list_ratio = 0.3

# auto completion
auto_completion_enabled = true

# enable tree view by default
tree_enabled = true

# preserve id3v1 tags
keep_id3v1 = false

# command to run automatically on startup
startup_command = "selectall && run reggae"

# show metadata icons when using a nerd font
field_icons_enabled = false

[appearance]
# symbols used in the explorer and metadata comparisons
selection_symbol = "✓"
change_symbol = "→"

# colors in hexadecimal format
text_fg = "#D9E0EB"
accent_fg = "#B2CDE3"
muted_fg = "#92A0B2"
success_fg = "#86EFAC"
danger_fg = "#FB7185"
focused_bg = "#334155"
border = "#AD97C2"
command_border = "#D4A35F"

[field_icons]
# example overrides for metadata field icons
artist = "@"
picture = "*"

[macros]
# example of macro. You can add as many as you want similarly.
reggae="edit genre=\"Reggae\""

[discogs.map]
# override built-in genre mapping (styles -> genres fallback)
genre = "{{ if release.styles && release.styles.size > 0; release.styles | joined; else; release.genres | joined; end }}"

# add or override any tag field
label = "{{ release.labels | array.map 'name' | joined }}"
country = "{{ release.country }}"
discogs_release_id = "{{ release.id }}"

[musicbrainz.map]
# override built-in genre mapping
genre = "{{ release.releasegroup.genres | array.map 'name' | joined }}"

# add or override any tag field
label = "{{ release.labelinfo | array.map 'label' | array.map 'name' | joined }}"
barcode = "{{ release.barcode }}"
musicbrainz_album_id = "{{ release.id }}"
```

Customize colors and selection and change symbols in `[appearance]`. Metadata field icons are disabled by default. Set `field_icons_enabled = true` in `[general]` if your terminal uses a Nerd Font. The `[field_icons]` section overrides individual metadata field icons; other fields keep their defaults.

If you have an existing config, move `selection_symbol` and `change_symbol` from `[general]` to `[appearance]` and rename `[color_scheme]` to `[appearance]`. Old settings in those locations are ignored.

The old `header_visible` setting has been removed. The status panel now shows common shortcuts.

`discogs.map` customizes how `:discogs` writes tags.

`musicbrainz.map` customizes how `:musicbrainz` (or `:mb`) writes tags.

Each key is a target tag field name, and each value is a template.
