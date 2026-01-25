#!/usr/bin/env bash
#
# Create .deb packages for Linux distributions.
#
# This script packages the published keystone-cli binary into .deb format
# for Debian/Ubuntu systems using nfpm.
#
# Usage:
#   ./scripts/package-deb.sh [version] [rid]
#
# Arguments:
#   version  Optional release version (e.g., 0.1.0).
#            Extracted from Keystone.Cli.csproj if omitted.
#   rid      Optional runtime identifier (linux-x64 or linux-arm64).
#            If omitted, packages both architectures.
#
# Examples:
#   ./scripts/package-deb.sh                    # Both archs, version from csproj
#   ./scripts/package-deb.sh 0.1.0              # Both archs, explicit version
#   ./scripts/package-deb.sh 0.1.0 linux-x64    # Single arch, explicit version
#   ./scripts/package-deb.sh linux-arm64        # Single arch, version from csproj
#
# Requirements:
#   - nfpm (brew install nfpm or go install github.com/goreleaser/nfpm/v2/cmd/nfpm@latest)
#   - Published binary: dotnet publish -c Release -r <rid>
#
# Exit codes:
#   0 - Package(s) created successfully
#   1 - Missing requirements or build failed
#   2 - Invalid arguments
#

set -euo pipefail

# Always run relative to the repo root.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
cd "${REPO_ROOT}"

VERSION=""
RID=""

usage() {
  echo "Usage: $(basename "$0") [version] [rid]" >&2
  echo "  version: Optional release version (e.g., 0.1.0)." >&2
  echo "           Extracted from Keystone.Cli.csproj if omitted; fails if not found." >&2
  echo "  rid:     Optional runtime identifier (linux-x64 or linux-arm64)." >&2
  echo "           If provided, only that RID package will be produced." >&2
  echo "" >&2
  echo "Examples:" >&2
  echo "  $(basename "$0")" >&2
  echo "  $(basename "$0") 0.1.0" >&2
  echo "  $(basename "$0") 0.1.0 linux-x64" >&2
  echo "  $(basename "$0") linux-arm64" >&2
}

if [[ $# -gt 2 ]]; then
  usage
  exit 2
fi

if [[ $# -eq 1 ]]; then
  # Support either "version" OR "rid" as the sole argument.
  if [[ "$1" =~ ^linux- ]]; then
    RID="$1"
  else
    VERSION="$1"
  fi
fi

if [[ $# -eq 2 ]]; then
  VERSION="$1"
  RID="$2"
fi

TFM="$("${SCRIPT_DIR}/get-tfm.sh")"

if [[ -z "$VERSION" ]]; then
  VERSION="$("${SCRIPT_DIR}/get-version.sh")"
fi

# Validate version format for safe use in filenames
VERSION="$("${SCRIPT_DIR}/validate-version.sh" "$VERSION")"

OUT_DIR="artifacts/release"

mkdir -p "$OUT_DIR"

# Check for nfpm
if ! command -v nfpm >/dev/null 2>&1; then
  echo "ERROR: nfpm is not installed." >&2
  echo "Install with: brew install nfpm" >&2
  echo "         or: go install github.com/goreleaser/nfpm/v2/cmd/nfpm@latest" >&2
  exit 1
fi

package() {
  local RID="$1"
  local ARCH

  case "$RID" in
    linux-x64)  ARCH="amd64" ;;
    linux-arm64) ARCH="arm64" ;;
    *)
      echo "ERROR: Unsupported RID for .deb packaging: $RID" >&2
      echo "Supported RIDs: linux-x64, linux-arm64" >&2
      exit 1
      ;;
  esac

  local PUBLISH_DIR="artifacts/bin/Keystone.Cli/Release/${TFM}/${RID}/publish"

  if [[ ! -d "$PUBLISH_DIR" ]]; then
    echo "ERROR: Publish directory not found: $PUBLISH_DIR" >&2
    echo "Run: dotnet publish ./src/Keystone.Cli/Keystone.Cli.csproj -c Release -r $RID" >&2
    exit 1
  fi

  if [[ ! -f "$PUBLISH_DIR/keystone-cli" ]]; then
    echo "ERROR: Expected binary not found: $PUBLISH_DIR/keystone-cli" >&2
    exit 1
  fi

  if [[ ! -f "$PUBLISH_DIR/appsettings.json" ]]; then
    echo "ERROR: Expected config not found: $PUBLISH_DIR/appsettings.json" >&2
    exit 1
  fi

  if [[ ! -f "docs/man/man1/keystone-cli.1" ]]; then
    echo "ERROR: Man page not found: docs/man/man1/keystone-cli.1" >&2
    exit 1
  fi

  if [[ ! -f "LICENSE" ]]; then
    echo "ERROR: LICENSE file not found" >&2
    exit 1
  fi

  local PACKAGE="${OUT_DIR}/keystone-cli_${VERSION}_${ARCH}.deb"

  echo "Building ${PACKAGE} (RID: ${RID}, ARCH: ${ARCH})"

  ARCH="$ARCH" VERSION="$VERSION" RID="$RID" TFM="$TFM" \
    nfpm package --packager deb --target "$PACKAGE"

  if command -v shasum >/dev/null 2>&1; then
    shasum -a 256 "$PACKAGE"
  else
    sha256sum "$PACKAGE"
  fi
}

if [[ -n "$RID" ]]; then
  package "$RID"
else
  package linux-x64
  package linux-arm64
fi

echo "Done."
