#!/usr/bin/env bash
set -euo pipefail

INSTALL_DIR="${1:-}"

if [[ -z "$INSTALL_DIR" ]]; then
  echo "Usage: $0 <install_dir>"
  exit 1
fi

REPO="cantti/tagselecta"

mkdir -p "$INSTALL_DIR"

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
cp "$BINARY_PATH" "$INSTALL_DIR/tagselecta"
chmod +x "$INSTALL_DIR/tagselecta"

echo "Installation complete. Files installed into: $INSTALL_DIR"
