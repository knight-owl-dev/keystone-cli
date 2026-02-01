#!/usr/bin/env bash
set -euo pipefail

#
# Verify a .deb package installs and runs correctly.
#
# This script is designed to run inside a minimal Debian/Ubuntu container.
# It is called by both CI workflows and the local test-package.sh script.
#
# Usage:
#   ./scripts/verify-deb-install.sh <path-to-deb>
#
# Exit codes:
#   0 - Verification passed
#   1 - Verification failed or invalid arguments
#

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 <path-to-deb>"
  echo "Error: Expected exactly 1 argument, got $#"
  exit 1
fi

DEB_FILE="$1"

if [[ ! -f "$DEB_FILE" ]]; then
  echo "ERROR: File not found: $DEB_FILE"
  exit 1
fi

echo "=== Installing dependencies ==="
apt-get update
apt-get install -y man-db

echo ""
echo "=== Installing package: $DEB_FILE ==="
apt-get install -y "$DEB_FILE"

echo ""
echo "=== Verifying installation ==="
which keystone-cli
ls -la /opt/keystone-cli/
ls -la /usr/local/bin/keystone-cli

echo ""
echo "=== Running keystone-cli info ==="
keystone-cli info

echo ""
echo "=== Checking man page ==="
man -w keystone-cli

echo ""
echo "=== Version check ==="
keystone-cli --version

echo ""
echo "==========================================="
echo "SUCCESS: Package verified"
echo "==========================================="
