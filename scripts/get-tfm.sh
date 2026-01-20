#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

PROPS_PATH="$REPO_ROOT/Directory.Build.props"

TFM="$(sed -n 's:.*<TargetFramework>\(.*\)</TargetFramework>.*:\1:p' "$PROPS_PATH" | head -n 1)"

if [[ -z "$TFM" ]]; then
  echo "ERROR: Could not read <TargetFramework> from $PROPS_PATH" >&2
  exit 1
fi

echo "$TFM"
