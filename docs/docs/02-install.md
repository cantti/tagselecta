## Install

You can install TagSelecta in one of these ways:

1. **Homebrew tap:** install via Homebrew from `cantti/homebrew-tagselecta`.
2. **AUR package (Arch Linux):** install `tagselecta-bin` with an AUR helper like `yay` or `paru`.
3. **Install script (automatic):** use the provided script to download and install the latest release.
4. **Download release (manual):** download from GitHub Releases and place the binary in your `PATH`.

### Option 1. Install via Homebrew

If you use Homebrew, install TagSelecta from the tap:

```sh
brew tap cantti/tagselecta
brew install tagselecta
```

### Option 2. Install from AUR (Arch Linux)

If you use Arch Linux (or an Arch-based distro), you can install the AUR package:

```sh
yay -S tagselecta-bin
```

Or with `paru`:

```sh
paru -S tagselecta-bin
```

### Option 3. Install via Script (Automatic Install)

> Tip: the same script can be used to update the installed version.

You can install the latest release automatically using the provided installer script (review the script before running
it):

```sh
wget -qO- https://raw.githubusercontent.com/cantti/tagselecta/main/install.sh | bash
```

This installs to `$HOME/.local/bin` by default.

For system-wide install:

```sh
wget -qO- https://raw.githubusercontent.com/cantti/tagselecta/main/install.sh | sudo bash -s -- --system
```

### Option 4. Download Release (Manual Install)

1. Go to the **[Releases page](https://github.com/cantti/tagselecta/releases)**
2. Download the latest archive for your system
3. Extract it
4. Move the binary into your preferred location (for example):

```sh
mv tagselecta "$HOME/.local/bin"
```

Ensure `"$HOME/.local/bin"` is in your `PATH`.
