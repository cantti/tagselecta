#!/usr/bin/env bash
set -euo pipefail

ARG="${1:-}"
INSTALL_DIR="$HOME/.local/bin"

if [[ "$ARG" == "--help" || "$ARG" == "-h" ]]; then
	echo "Usage: $0 [--system|<install_dir>]"
	echo "  default: $HOME/.local/bin"
	echo "  --system: /usr/local/bin (may require sudo)"
	exit 0
fi

if [[ "$ARG" == "--system" ]]; then
	INSTALL_DIR="/usr/local/bin"
elif [[ -n "$ARG" ]]; then
	INSTALL_DIR="$ARG"
fi

REPO="cantti/tagselecta"

if ! mkdir -p "$INSTALL_DIR" 2>/dev/null; then
	echo "Cannot create install dir: $INSTALL_DIR"
	if [[ "$INSTALL_DIR" == "/usr/local/bin" ]]; then
		echo "Try: sudo bash install.sh --system"
	fi
	exit 1
fi

OS=$(uname -s | tr '[:upper:]' '[:lower:]')

case "$OS" in
linux)
	OS="linux"
	;;
darwin)
	OS="osx"
	;;
*)
	echo "Unsupported OS: $OS"
	exit 1
	;;
esac

ARCH=$(uname -m)

case "$ARCH" in
x86_64 | amd64)
	ARCH="x64"
	;;
arm64 | aarch64)
	ARCH="arm64"
	;;
*)
	echo "Unsupported architecture: $ARCH"
	exit 1
	;;
esac

# Expected asset format: tagselecta-<os>-<arch>.zip
ASSET_PREFIX="tagselecta-$OS-$ARCH"
echo "Detected platform: $OS-$ARCH"
echo "Looking for asset prefix: $ASSET_PREFIX"

echo "Fetching latest release for $REPO..."
API_URL="https://api.github.com/repos/$REPO/releases/latest"
JSON=$(curl -sL "$API_URL")

# Select correct binary asset
DOWNLOAD_URL=$(echo "$JSON" | grep "browser_download_url" | cut -d '"' -f 4 | grep "$ASSET_PREFIX" || true)

if [[ -z "$DOWNLOAD_URL" ]]; then
	echo "No matching asset found for platform: $ASSET_PREFIX"
	exit 1
fi

FILENAME=$(basename "$DOWNLOAD_URL")
TMPDIR=$(mktemp -d)

echo "Downloading $FILENAME ..."
curl -L "$DOWNLOAD_URL" -o "$TMPDIR/$FILENAME"

echo "Extracting..."
unzip -q "$TMPDIR/$FILENAME" -d "$TMPDIR"

echo "Installing into $INSTALL_DIR..."

# Find the binary inside the extracted folder
BINARY_PATH=$(find "$TMPDIR" -type f -maxdepth 3 -name "tagselecta" | head -n 1)

if [[ -z "$BINARY_PATH" ]]; then
	echo "Error: Could not find the tagselecta binary in the archive."
	exit 1
fi

echo "Installing tagselecta → $INSTALL_DIR/tagselecta"
if [[ ! -w "$INSTALL_DIR" ]]; then
	echo "Install dir is not writable: $INSTALL_DIR"
	if [[ "$INSTALL_DIR" == "/usr/local/bin" ]]; then
		echo "Try: sudo bash install.sh --system"
	fi
	exit 1
fi

cp "$BINARY_PATH" "$INSTALL_DIR/tagselecta"
chmod +x "$INSTALL_DIR/tagselecta"

echo "Installation complete. Files installed into: $INSTALL_DIR"
