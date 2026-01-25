#!/usr/bin/env bash
#
# Validate a .NET runtime identifier (RID) for safe use in scripts.
#
# Accepts standard RIDs used by this project: osx-arm64, osx-x64, linux-x64, linux-arm64.
# Use --linux to restrict to Linux RIDs only (for .deb packaging).
#
# Usage:
#   ./scripts/validate-rid.sh [--linux] <rid>
#
# Examples:
#   ./scripts/validate-rid.sh linux-x64        # Accepts any project RID
#   ./scripts/validate-rid.sh --linux linux-x64  # Accepts only Linux RIDs
#
# Exit codes:
#   0 - Valid RID
#   1 - Invalid RID or missing argument
#

set -euo pipefail

LINUX_ONLY=false

if [[ $# -ge 1 && "$1" == "--linux" ]]; then
    LINUX_ONLY=true
    shift
fi

if [[ $# -ne 1 ]]; then
    echo "Usage: $(basename "$0") [--linux] <rid>" >&2
    exit 1
fi

RID="$1"

if [[ "$LINUX_ONLY" == true ]]; then
    case "$RID" in
        linux-x64|linux-arm64)
            echo "$RID"
            ;;
        *)
            echo "ERROR: Invalid Linux RID: $RID" >&2
            echo "Supported Linux RIDs: linux-x64, linux-arm64" >&2
            exit 1
            ;;
    esac
else
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
fi
