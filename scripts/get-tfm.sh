#!/usr/bin/env bash
#
# Extract the target framework moniker (TFM) from Directory.Build.props.
#
# Reads the <TargetFramework> element and outputs the value (e.g., "net10.0").
# Used by build scripts to locate publish output directories.
#
# Usage:
#   ./scripts/get-tfm.sh
#
# Output:
#   Prints the TFM to stdout (e.g., "net10.0")
#
# Exit codes:
#   0 - Success
#   1 - Could not read TargetFramework from Directory.Build.props
#

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
