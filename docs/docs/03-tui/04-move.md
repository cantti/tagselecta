# Move command

Moves/renames files using a template.

Example. Move all audio files to a folder structure based on the album and year.

```
:move template="../{{ year }} - {{ album }}/{{ filename }}.{{ext}}"
```

Options:

- `template` (`t`): Destination template (e.g. `../{{ year }} - {{ album }}/{{ filename }}.{{ext}}`)
- `keepemptydirs` (`k`):Keep empty source directories after moving
- `donotmoveother` (`d`): Only move the audio files (don’t move other files in the folder)
