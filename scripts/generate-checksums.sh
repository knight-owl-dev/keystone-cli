#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

DIST_DIR=""

usage() {
  echo "Usage: $(basename "$0") [dist-dir]" >&2
  echo "  dist-dir: Directory containing release artifacts (.tar.gz, .deb files)." >&2
  echo "            Defaults to 'artifacts/release' if omitted." >&2
  echo "" >&2
  echo "Outputs (in artifacts/release/):" >&2
  echo "  checksums.txt    - SHA256 checksums in GNU coreutils format" >&2
  echo "  release-body.md  - Markdown-formatted checksums for GitHub release" >&2
  echo "" >&2
  echo "Examples:" >&2
  echo "  $(basename "$0")" >&2
  echo "  $(basename "$0") dist" >&2
  echo "  $(basename "$0") artifacts/release" >&2
}

if [[ $# -gt 1 ]]; then
  usage
  exit 2
fi

# Resolve DIST_DIR to absolute path BEFORE cd to REPO_ROOT
if [[ $# -eq 1 ]]; then
  if [[ "$1" == "-h" || "$1" == "--help" ]]; then
    usage
    exit 0
  fi
  if [[ ! -d "$1" ]]; then
    echo "ERROR: Directory not found: $1" >&2
    exit 1
  fi
  DIST_DIR="$(cd "$1" && pwd)"
fi

# Now safe to change to repo root
cd "${REPO_ROOT}"

# Default uses repo-relative path
if [[ -z "$DIST_DIR" ]]; then
  DIST_DIR="$REPO_ROOT/artifacts/release"
  if [[ ! -d "$DIST_DIR" ]]; then
    echo "ERROR: Directory not found: $DIST_DIR" >&2
    exit 1
  fi
fi

OUT_DIR="artifacts/release"

mkdir -p "$OUT_DIR"

# Find all release files (.tar.gz and .deb)
files=$(cd "$DIST_DIR" && find . -maxdepth 3 -type f \( -name '*.tar.gz' -o -name '*.deb' \) -print | sed 's|^\./||')

if [[ -z "$files" ]]; then
  echo "ERROR: No release files found under $DIST_DIR/" >&2
  exit 1
fi

echo "Release assets:"
echo "$files" | sort

# Compute checksums in GNU coreutils format (checksum first, compatible with sha256sum -c)
# Output uses basenames only for cleaner checksums.txt
(cd "$DIST_DIR" && sha256sum $files) | while read -r sum file; do
  echo "$sum  $(basename "$file")"
done | sort > "$OUT_DIR/checksums.txt"

echo ""
echo "$OUT_DIR/checksums.txt:"
cat "$OUT_DIR/checksums.txt"

# Generate release body with markdown-formatted checksums
{
  echo ""
  echo "## SHA256 Checksums"
  echo ""
  echo '```'
  cat "$OUT_DIR/checksums.txt"
  echo '```'
} > "$OUT_DIR/release-body.md"

echo ""
echo "$OUT_DIR/release-body.md:"
cat "$OUT_DIR/release-body.md"

echo ""
echo "Done."
