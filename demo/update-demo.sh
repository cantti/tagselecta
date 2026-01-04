#!/bin/bash
set -e

dir="$PWD"
TMPDIR=$(mktemp -d)

# Publish to temp directory to avoid polluting cwd
dotnet publish -c Release --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:DebugType=None -p:DebugSymbols=false \
  -o "$TMPDIR" ../TagSelecta.Cli

# Copy additional assets
cp -r Album "$TMPDIR"
cp demo.tape "$TMPDIR"

cd "$TMPDIR"

# Generate animated terminal demo
vhs demo.tape

# Move resulting GIF back to original directory
mv demo.mp4 "$dir/"

cd "$dir"
rm -rf "$TMPDIR"
