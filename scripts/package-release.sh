#!/usr/bin/env bash
set -euo pipefail

# Always run relative to the repo root.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
cd "${REPO_ROOT}"

VERSION="${1:-0.1.0}"
TFM="net10.0"
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

  local ARCHIVE="${OUT_DIR}/keystone-cli_${VERSION}_${RID}.tar.gz"

  echo "Packaging ${ARCHIVE} (RID: ${RID})"

  tar -C "$PUBLISH_DIR" \
    -czf "$ARCHIVE" \
    keystone-cli \
    appsettings.json \
    -C "$REPO_ROOT/docs/man/man1" keystone-cli.1

  if command -v shasum >/dev/null 2>&1; then
    shasum -a 256 "$ARCHIVE"
  else
    sha256sum "$ARCHIVE"
  fi
}

package osx-arm64
package osx-x64

echo "Done."
