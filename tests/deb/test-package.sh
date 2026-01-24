#!/usr/bin/env bash
#
# Test a locally-built .deb package in a Docker container.
#
# This script verifies that the .deb package installs and runs correctly
# on minimal Debian/Ubuntu systems before publishing to the apt repository.
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

set -euo pipefail

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

if [[ ! -f "$DEB_FILE" ]]; then
    echo "ERROR: File not found: $DEB_FILE"
    exit 1
fi

DEB_FILENAME=$(basename "$DEB_FILE")

echo "Testing .deb package: $DEB_FILE"
echo "Image: $IMAGE"
echo "==========================================="

docker run --rm -v "$(cd "$(dirname "$DEB_FILE")" && pwd):/deb:ro" "$IMAGE" bash -c "
set -e

echo '=== Installing dependencies ==='
apt-get update
apt-get install -y man-db

echo ''
echo '=== Installing .deb package ==='
apt-get install -y /deb/$DEB_FILENAME

echo ''
echo '=== Verifying installation ==='
which keystone-cli
ls -la /opt/keystone-cli/
ls -la /usr/local/bin/keystone-cli

echo ''
echo '=== Running keystone-cli info ==='
keystone-cli info

echo ''
echo '=== Checking man page ==='
man -w keystone-cli

echo ''
echo '=== Testing basic commands ==='
keystone-cli --version

echo ''
echo '==========================================='
echo 'SUCCESS: Package installed and working'
echo '==========================================='
"
