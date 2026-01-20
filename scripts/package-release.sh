#!/usr/bin/env bash
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
  echo "           Defaults to the current <Version> value in Keystone.Cli.csproj if omitted." >&2
  echo "  rid:     Optional runtime identifier (e.g., osx-arm64, osx-x64, linux-x64)." >&2
  echo "           If provided, only that RID archive will be produced." >&2
  echo "" >&2
  echo "Examples:" >&2
  echo "  $(basename "$0")" >&2
  echo "  $(basename "$0") 0.1.0" >&2
  echo "  $(basename "$0") 0.1.0 osx-arm64" >&2
  echo "  $(basename "$0") osx-arm64" >&2
}

if [[ $# -gt 2 ]]; then
  usage
  exit 2
fi

if [[ $# -eq 1 ]]; then
  # Support either "version" OR "rid" as the sole argument.
  if [[ "$1" =~ ^(osx|win|linux)(-|$) ]]; then
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
  # Best-effort: extract <Version>...</Version> from the CLI project file.
  # (Keeps this script dependency-free; falls back to 0.1.0 if not found.)
  if [[ -f "./src/Keystone.Cli/Keystone.Cli.csproj" ]]; then
    VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' ./src/Keystone.Cli/Keystone.Cli.csproj | head -n 1)"
  fi

  if [[ -z "$VERSION" ]]; then
    VERSION="0.1.0"
  fi
fi

OUT_DIR="artifacts/release"

mkdir -p "$OUT_DIR"

package() {
  local RID="$1"

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
    echo "ERROR: LICENSE file not found: LICENSE" >&2
    exit 1
  fi

  local ARCHIVE="${OUT_DIR}/keystone-cli_${VERSION}_${RID}.tar.gz"

  echo "Packaging ${ARCHIVE} (RID: ${RID})"

  tar -C "$PUBLISH_DIR" \
    -czf "$ARCHIVE" \
    keystone-cli \
    appsettings.json \
    -C "$REPO_ROOT" LICENSE \
    -C "$REPO_ROOT/docs/man/man1" keystone-cli.1

  if command -v shasum >/dev/null 2>&1; then
    shasum -a 256 "$ARCHIVE"
  else
    sha256sum "$ARCHIVE"
  fi
}

if [[ -n "$RID" ]]; then
  package "$RID"
else
  package osx-arm64
  package osx-x64
  package linux-x64
  package linux-arm64
fi

echo "Done."
