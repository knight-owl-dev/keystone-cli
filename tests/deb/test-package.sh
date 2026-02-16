#!/usr/bin/env bash
set -euo pipefail

#
# Test a locally-built .deb package in a Docker container.
#
# This script runs verify-deb-install.sh inside a Docker container to verify
# that the .deb package installs and runs correctly on minimal Debian/Ubuntu
# systems before publishing to the apt repository.
#
# Usage:
#   ./tests/deb/test-package.sh <path-to-deb> [image]
#
# Examples:
#   ./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.10_amd64.deb
#   ./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.10_arm64.deb debian:bookworm-slim
#   ./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.10_amd64.deb ubuntu:24.04
#
# Requirements:
#   - Docker must be installed and running
#   - The .deb file must exist at the specified path
#
# Exit codes:
#   0 - All tests passed
#   1 - Test failed or invalid arguments
#

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

if [[ $# -lt 1 ]]; then
  echo "Usage: $0 <path-to-deb> [image]"
  echo ""
  echo "Examples:"
  echo "  $0 artifacts/release/keystone-cli_0.1.10_amd64.deb"
  echo "  $0 artifacts/release/keystone-cli_0.1.10_arm64.deb ubuntu:24.04"
  exit 1
fi

DEB_FILE="$1"
IMAGE="${2:-debian:bookworm-slim}"

if [[ ! -f "${DEB_FILE}" ]]; then
  echo "ERROR: File not found: ${DEB_FILE}" >&2
  exit 1
fi

DEB_DIR="$(cd "$(dirname "${DEB_FILE}")" && pwd)"
DEB_FILENAME="$(basename "${DEB_FILE}")"

echo "Testing .deb package: ${DEB_FILE}"
echo "Image: ${IMAGE}"

docker run --rm \
  -v "${DEB_DIR}:/deb:ro" \
  -v "${REPO_ROOT}/scripts:/scripts:ro" \
  "${IMAGE}" \
  /scripts/verify-deb-install.sh "/deb/${DEB_FILENAME}"
