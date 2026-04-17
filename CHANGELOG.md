## [2.0.0] - 2026-04-17

### Features

- Add musicbrainz support, rework discogs to use scriban template engine (#13)
- More discogs models
- [**breaking**] Make extra field editing simpler and unify tag storage (#14)
- [**breaking**] Rename disc/track fields to discnumber/tracknumber and drop most edit short flags
- Update installer
- Improve musicbrainz mapping (#15)
- Improve id3v1 handling, add new setting `keep_id3v1`

### Fixes

- Require {{ }} in find command
- Fix deploy pages

### Docs

- Auto-update changelog
- Docs update
- Auto-update docs
- Docs update
- Auto-update docs
- Auto-update docs

## [1.4.0] - 2026-03-28

### Features

- Ignore extra properties in toml

### Docs

- Aur package
- Auto-update changelog

## [1.3.2] - 2026-03-27

### Fixes

- Remove sha from version command

## [1.3.1] - 2026-03-27

### Fixes

- Remove sha from version

## [1.3.0] - 2026-03-27

### Features

- Add setting to show tree view by default

## [1.2.0] - 2026-03-23

### Features

- Ogg support

## [1.1.0] - 2026-02-25

### Docs

- Docs
- Docs
- Docs
- Docs

## [1.0.4] - 2026-02-21

### Docs

- Docs
- Docs cicd

## [1.0.2] - 2026-02-19

### Features

- *(tui)* Add PictureWidget and TogglePictureCommand with hotkey 'p'

## [1.0.1] - 2026-02-19

### Fixes

- Normalize bpm field key to lowercase

### Docs

- Auto-update README CLI help [skip ci]

## [0.11.17] - 2026-02-14

### Fixes

- *(config)* Seed default TOML when creating missing config file

### Docs

- Auto-update README CLI help [skip ci]

## [0.11.16] - 2026-02-13

### Features

- *(cli)* Add --yes flag to skip confirmation prompt

## [0.11.15] - 2026-02-07

### Fixes

- Adjust `filesContentSize` calculation and improve error handling in `TagDataFormatter` and UI widget

## [0.11.14] - 2026-02-07

### Docs

- Auto-update README CLI help [skip ci]

## [0.11.11] - 2026-02-04

### Docs

- Auto-update README CLI help [skip ci]

## [0.11.10] - 2026-02-03

### Features

- Improve macros support

### Fixes

- *(move)* Use current tag data when formatting moved file name

### Docs

- Huget reamde update
- Update README with new tag field and option syntax

## [0.11.9] - 2026-01-29

### Features

- Debug config

## [0.11.8] - 2026-01-29

### Features

- Add new TUI commands and hotkey token management
- Integrate configuration system for macro management and TOML parsing

## [0.11.7] - 2026-01-28

### Features

- Add zero-padded formatting for disc and track numbers
- Hotkey handling improvements

### Docs

- Auto-update README CLI help [skip ci]

## [0.11.3] - 2026-01-18

### Fixes

- Minor logic fixes

### Performance

- Optimize tagdata print performance

### Docs

- Auto-update README CLI help [skip ci]

## [0.11.2] - 2026-01-17

### Fixes

- File list widget fixes
- Discogs actions
- Minor cli fixes

### Docs

- Auto-update README CLI help [skip ci]

## [0.11.1] - 2026-01-17

### Features

- Support cancellation in audio file scanning/reading
- Improve move operation

### Fixes

- Treelistwidget

## [0.11.0] - 2026-01-15

### Features

- Write in cli command

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.28] - 2026-01-12

### Features

- Make discogs id custom field

### Fixes

- Small fixes
- Discogs
- Mindor discogs action fixes

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.26] - 2026-01-12

### Features

- Update demo
- Add validation for base settings in ExecuteTagDataActionCommand
- Move action

### Fixes

- Command scheduling logic

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.25] - 2026-01-11

### Features

- Rename project
- Add missing app project
- Add SelectDirCommand to select operations in the same directory in TuiCommands

### Fixes

- *(cli)* Update ClearCustom logic in EditSettings and register AutoTrackAction in Registration
- Update UndoCommand to use SelectedOperations instead of Operations in TuiCommands

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.24] - 2026-01-04

### Docs

- Demo update

## [0.10.22] - 2026-01-03

### Fixes

- *(cli)* Report informational version and set default project Version

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.21] - 2026-01-03

### Features

- *(release)* Embed tag-derived version in CLI and display app version

## [0.10.20] - 2026-01-03

### Fixes

- *(cli)* Improve performance when filtering

## [0.10.19] - 2026-01-03

### Features

- *(cli)* Add interactive filter toggle and only write changed files

## [0.10.18] - 2026-01-03

### Features

- *(cli)* Use colored markup for keybinding help line
- *(cli)* Process tag data in parallel with progress display

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.17] - 2025-12-29

### Features

- *(cli)* Add --yes option to skip interactive prompts

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.16] - 2025-12-28

### Features

- Better error handling
- More logical command order
- *(docs)* Update examples to use -s alias for custom tags

### Docs

- Auto-update README CLI help [skip ci]
- Update readme

## [0.10.15] - 2025-12-27

### Features

- Rename write to edit

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.14] - 2025-12-27

### Features

- Write action better space support in custom tags

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.13] - 2025-12-27

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.11] - 2025-12-24

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.10] - 2025-11-30

### Features

- Better picture handling
- Better custom fields usage

### Docs

- Auto-update README CLI help [skip ci]
- Docs
- Docs
- Docs
- Docs
- Readme

## [0.10.9] - 2025-11-29

### Docs

- Auto-update README CLI help [skip ci]
- Docs

## [0.10.8] - 2025-11-25

### Fixes

- Fix add noansi env var again

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.7] - 2025-11-25

### Fixes

- Fix remove custom no color flag

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.6] - 2025-11-25

### Fixes

- Add demo

### Docs

- Auto-update README CLI help [skip ci]

## [0.10.0] - 2025-11-15

### Docs

- Auto-update README CLI help [skip ci]

## [0.9.4] - 2025-11-10

### Docs

- Auto-update README CLI help [skip ci]

## [0.9.3] - 2025-11-09

### Docs

- Auto-update README CLI help [skip ci]

## [0.9.2] - 2025-11-07

### Docs

- Auto-update README CLI help [skip ci]

## [0.9.1] - 2025-11-07

### Docs

- Auto-update README CLI help [skip ci]

## [0.9.0] - 2025-11-06

### Docs

- Auto-update README CLI help [skip ci]

## [0.8.0] - 2025-11-05

### Docs

- Auto-update README CLI help [skip ci]

## [0.7.1] - 2025-11-03

### Docs

- Auto-update README CLI help [skip ci]

## [0.7.0] - 2025-11-02

### Docs

- Auto-update README CLI help [skip ci]

## [0.6.0] - 2025-11-02

### Docs

- Auto-update README CLI help [skip ci]

## [0.5.0] - 2025-11-01

### Docs

- Auto-update README CLI help [skip ci]

## [0.4.3] - 2025-11-01

### Docs

- Auto-update README CLI help [skip ci]

## [0.3.3] - 2025-10-31

### Fixes

- Fix update-readme workflow

## [0.3.2] - 2025-10-31

### Fixes

- Fix update-readme workflow

## [0.3.1] - 2025-10-31

### Fixes

- Fix write action

## [0.3.0] - 2025-10-31

### Docs

- Auto-update README CLI help [skip ci]

## [0.2.1] - 2025-10-31

### Features

- Renamedir command, refactoring
- Add templates to rename dir command
- Use SmartFormat
- More tags, use mapperly

### Docs

- Update readme

## [0.1.1] - 2025-10-26

### Fixes

- Fix album artist command

### Docs

- Update readme
- Update readme
- Update readme

