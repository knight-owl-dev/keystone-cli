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

if [[ ! -f "${DEB_FILE}" ]]; then
  echo "ERROR: File not found: ${DEB_FILE}" >&2
  exit 1
fi

# Install the package (dependencies are declared in the .deb)
apt-get update -qq > /dev/null
apt-get install -y -qq man-db > /dev/null
apt-get install -y -qq "${DEB_FILE}" > /dev/null

# Verify
echo -n "Binary exists at /opt/keystone-cli/keystone-cli..." \
  && test -x /opt/keystone-cli/keystone-cli \
  && echo " OK"

echo -n "Symlink exists at /usr/local/bin/keystone-cli..." \
  && test -L /usr/local/bin/keystone-cli \
  && echo " OK"

echo -n "keystone-cli --version..." \
  && keystone-cli --version > /dev/null \
  && echo " OK"

echo -n "keystone-cli info..." \
  && keystone-cli info > /dev/null \
  && echo " OK"

echo -n "man -w keystone-cli..." \
  && man -w keystone-cli > /dev/null \
  && echo " OK"

echo -n "Bash completion exists..." \
  && test -f /usr/share/bash-completion/completions/keystone-cli \
  && echo " OK"

echo -n "Zsh completion exists..." \
  && test -f /usr/share/zsh/vendor-completions/_keystone-cli \
  && echo " OK"
