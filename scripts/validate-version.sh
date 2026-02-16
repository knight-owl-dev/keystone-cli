#!/usr/bin/env bash
set -euo pipefail

#
# Validate a version string for safe use in filenames and shell commands.
#
# Accepts semantic versions: MAJOR.MINOR.PATCH with optional prerelease suffix.
# Examples: 1.0.0, 0.2.0, 1.0.0-beta.1, 2.0.0-rc.1
#
# Usage:
#   ./scripts/validate-version.sh <version>
#
# Exit codes:
#   0 - Valid version
#   1 - Invalid version or missing argument
#

if [[ $# -ne 1 ]]; then
  echo "Usage: $(basename "$0") <version>" >&2
  exit 1
fi

VERSION="$1"

# Semver pattern: MAJOR.MINOR.PATCH with optional -prerelease.identifier
# Allows: 1.0.0, 0.2.0, 1.0.0-beta.1, 2.0.0-rc.1, 1.0.0-alpha
if [[ ! "${VERSION}" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*)?$ ]]; then
  echo "ERROR: Invalid version format: ${VERSION}" >&2
  echo "Expected: MAJOR.MINOR.PATCH (e.g., 1.0.0) or MAJOR.MINOR.PATCH-prerelease (e.g., 1.0.0-beta.1)" >&2
  exit 1
fi

echo "${VERSION}"
