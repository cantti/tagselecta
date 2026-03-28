# Install

You can install TagSelecta in one of these ways:

1. **Download release (manual):** download from GitHub Releases and place the binary in your `PATH`.
2. **Install script (automatic):** use the provided script to download and install the latest release.
3. **AUR package (Arch Linux):** install `tagselecta-bin` with an AUR helper like `yay` or `paru`.

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

> Tip: the same script can be used to update the installed version.

You can install the latest release automatically using the provided installer script (review the script before running it):

```sh
wget -qO- https://raw.githubusercontent.com/cantti/tagselecta/main/install.sh | bash -s "$HOME/.local/bin"
```

This will:

* download the latest release,
* extract it,
* install it into the directory you provide (default recommended: `$HOME/.local/bin`).

### Option 3. Install from AUR (Arch Linux)

If you use Arch Linux (or an Arch-based distro), you can install the AUR package:

```sh
yay -S tagselecta-bin
```

Or with `paru`:

```sh
paru -S tagselecta-bin
```
