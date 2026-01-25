#!/usr/bin/env bash
#
# Validate a Debian architecture for safe use in scripts.
#
# Accepts architectures used by this project: amd64, arm64.
#
# Usage:
#   ./scripts/validate-arch.sh <arch>
#
# Exit codes:
#   0 - Valid architecture
#   1 - Invalid architecture or missing argument
#

set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Usage: $(basename "$0") <arch>" >&2
    exit 1
fi

ARCH="$1"

case "$ARCH" in
    amd64|arm64)
        echo "$ARCH"
        ;;
    *)
        echo "ERROR: Invalid architecture: $ARCH" >&2
        echo "Supported architectures: amd64, arm64" >&2
        exit 1
        ;;
esac
