# Settings

Config file location: 

```
~/.config/tagselecta/config.toml
```

Default is generated on the first run.

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
tree_enabled = false

# preserve id3v1 tags
keep_id3v1 = false

# show header in tui mode (app name and common shortcuts)
header_visible = true

# show header in tui mode (app name and common shortcuts)
header_visible = true

# select all files on startup
select_all_on_startup = false

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

`discogs.map` customizes how `:discogs` writes tags.

`musicbrainz.map` customizes how `:musicbrainz` (or `:mb`) writes tags.

Each key is a target tag field name, and each value is a template.
