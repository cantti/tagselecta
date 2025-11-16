#!/bin/bash

# get latest tag (strip leading "v" if present)
latest=$(git describe --tags --abbrev=0 2>/dev/null || echo "0.0.0")
latest_no_v="${latest#v}"

IFS='.' read -r major minor patch <<<"$latest_no_v"

case "$1" in
patch | "")
  patch=$((patch + 1))
  ;;
minor)
  minor=$((minor + 1))
  patch=0
  ;;
major)
  major=$((major + 1))
  minor=0
  patch=0
  ;;
*)
  echo "Usage: $0 [patch|minor|major]"
  exit 1
  ;;
esac

new="v$major.$minor.$patch"

git tag -a "$new" -m "Release $new"
echo "Created tag $new"
echo "You can run now: git push --tags"
