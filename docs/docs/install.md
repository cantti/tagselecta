---
sidebar_position: 2
---

# Install

### Option 1. Download Release (Manual Install)

1. Go to the **[Releases page](https://github.com/cantti/tagselecta/releases)**
2. Download the latest archive for your system
3. Extract it
4. Move the binary into your preferred location (for example):

```sh
mv tagselecta "$HOME/.local/bin"
```

Ensure `"$HOME/.local/bin"` is in your `PATH`.

### Option 2. Install via Script (Automatic Install)

You can install the latest release automatically using the provided installer script (review the script before running it):

```sh
wget -qO- https://raw.githubusercontent.com/cantti/tagselecta/main/install.sh | bash -s "$HOME/.local/bin"
```

This will:

* download the latest release,
* extract it,
* install it into the directory you provide (default recommended: `$HOME/.local/bin`).
* 
> Tip: the same script can be used to update the installed version.
