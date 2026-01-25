#!/usr/bin/env bash
#
# Validate a .NET runtime identifier (RID) for safe use in scripts.
#
# Accepts standard RIDs used by this project: osx-arm64, osx-x64, linux-x64, linux-arm64.
#
# Usage:
#   ./scripts/validate-rid.sh <rid>
#
# Exit codes:
#   0 - Valid RID
#   1 - Invalid RID or missing argument
#

set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Usage: $(basename "$0") <rid>" >&2
    exit 1
fi

RID="$1"

case "$RID" in
    osx-arm64|osx-x64|linux-x64|linux-arm64)
        echo "$RID"
        ;;
    *)
        echo "ERROR: Invalid RID: $RID" >&2
        echo "Supported RIDs: osx-arm64, osx-x64, linux-x64, linux-arm64" >&2
        exit 1
        ;;
esac
