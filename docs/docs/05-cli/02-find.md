# Find command

Find files by metadata. The query is a Scriban template. Matching results are printed as full file paths, one per line.

```bash
tagselecta find . --query "{{ title | string.downcase | string.contains 'dub' }}"
```

## Run a command for each match

Use `--exec` to run an external program once per matching file. Put its arguments after `--` and use `{}` for the file path:

```bash
tagselecta find . --query "{{ genre == 'Reggae' }}" --exec ffprobe -- "{}"
tagselecta find . --query "{{ genre == 'Reggae' }}" --exec tagselecta -- edit "{}" --genre Rock --yes
```

If no argument contains `{}`, the file path is appended automatically. Arguments are passed directly to the program without a shell, so paths containing spaces are safe. Shell operators such as pipes and redirection are not interpreted.

When using `--exec`, the child program's output is shown instead of the usual list of matching paths. Execution stops and returns the child's exit code if a command fails. No commands run when there are no matches.
