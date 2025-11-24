#!/bin/bash
set -e

# Recreate clean temporary workspace
TMP=tmp-demo
rm -rf "$TMP"
mkdir "$TMP"

# Copy required demo assets
cp -r "demo/Cantti - Echoes of the Woods" "$TMP"
cp demo/{tagselecta,demo.tape} "$TMP"

# Run VHS to generate the GIF
cd "$TMP"
vhs demo.tape

# Move output back to project root
mv demo.gif ..
