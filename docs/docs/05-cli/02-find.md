# Find command

Find files by metadata. The query is a Scriban template. Matching results are printed as full file paths, one per line.

```bash
tagselecta find . --query "{{ title | string.downcase | string.contains 'dub' }}"
```
