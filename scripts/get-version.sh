#!/usr/bin/env bash
set -euo pipefail

#
# Extract the project version from Keystone.Cli.csproj.
#
# Reads the <Version> element and outputs the value (e.g., "0.1.9").
# Used by build and packaging scripts to determine the release version.
#
# Usage:
#   ./scripts/get-version.sh
#
# Output:
#   Prints the version to stdout (e.g., "0.1.9")
#
# Exit codes:
#   0 - Success
#   1 - Could not read Version from csproj
#

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

CSPROJ_PATH="$REPO_ROOT/src/Keystone.Cli/Keystone.Cli.csproj"

VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$CSPROJ_PATH" | head -n 1)"

if [[ -z "$VERSION" ]]; then
  echo "ERROR: Could not read <Version> from $CSPROJ_PATH" >&2
  exit 1
fi

echo "$VERSION"
