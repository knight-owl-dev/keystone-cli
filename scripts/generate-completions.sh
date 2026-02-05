#!/usr/bin/env bash
set -euo pipefail

#
# Generate shell completion scripts for bash and zsh.
#
# This script runs the keystone-cli binary with --completion flag to generate
# completion scripts for distribution. The scripts are identical regardless of
# platform (based on command structure, not runtime).
#
# Usage:
#   ./scripts/generate-completions.sh <binary-path> <output-dir>
#
# Arguments:
#   binary-path  Path to the keystone-cli binary.
#   output-dir   Directory where completion scripts will be written.
#
# Output files:
#   keystone-cli.bash  Bash completion script
#   _keystone-cli      Zsh completion script (underscore prefix is zsh convention)
#
# Examples:
#   ./scripts/generate-completions.sh ./keystone-cli ./completions
#   ./scripts/generate-completions.sh artifacts/bin/.../publish/keystone-cli completions
#
# Exit codes:
#   0 - Scripts generated successfully
#   1 - Missing binary or generation failed
#   2 - Invalid arguments
#

usage() {
  echo "Usage: $(basename "$0") <binary-path> <output-dir>" >&2
  echo "  binary-path: Path to the keystone-cli binary." >&2
  echo "  output-dir:  Directory where completion scripts will be written." >&2
}

if [[ $# -ne 2 ]]; then
  usage
  exit 2
fi

BINARY="$1"
OUTPUT_DIR="$2"

if [[ ! -f "${BINARY}" ]]; then
  echo "ERROR: Binary not found: ${BINARY}" >&2
  exit 1
fi

if [[ ! -x "${BINARY}" ]]; then
  echo "ERROR: Binary is not executable: ${BINARY}" >&2
  exit 1
fi

mkdir -p "${OUTPUT_DIR}"

echo "Generating bash completion..."
"${BINARY}" --completion bash > "${OUTPUT_DIR}/keystone-cli.bash"

echo "Generating zsh completion..."
"${BINARY}" --completion zsh > "${OUTPUT_DIR}/_keystone-cli"

echo "Completion scripts generated in ${OUTPUT_DIR}:"
ls -la "${OUTPUT_DIR}"
