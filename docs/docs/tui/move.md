---
sidebar_position: 3
---

# Move command

```
:move t=...
```

Moves/renames files using a template.

- `template` (`t`): Destination template (e.g. `../{{ year }} - {{ album }}/{{ filename }}.{{ext}}`)
- `keepemptydirs` (`k`):Keep empty source directories after moving
- `donotmoveother` (`d`): Only move the audio files (don’t move other files in the folder)
