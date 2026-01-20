#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

CSPROJ_PATH="$REPO_ROOT/src/Keystone.Cli/Keystone.Cli.csproj"

VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$CSPROJ_PATH" | head -n 1)"

if [[ -z "$VERSION" ]]; then
  echo "ERROR: Could not read <Version> from $CSPROJ_PATH" >&2
  exit 1
fi

echo "$VERSION"
