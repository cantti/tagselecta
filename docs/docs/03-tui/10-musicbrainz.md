# MusicBrainz command

Download metadata from MusicBrainz.

If front cover art exists, TagSelecta also downloads it.

```sh
:musicbrainz release="https://musicbrainz.org/release/b2c254f7-6828-45b8-b9b3-0d8d5c986088"
```

`release` can be:
- a **release** URL (`.../release/<mbid>`)
- a **release id** (MBID)

Field mapping can be customized using [settings](../06-settings.md).
