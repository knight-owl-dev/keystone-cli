#!/usr/bin/env bash
#
# Build and test all .deb packages locally.
#
# This script builds .deb packages for all Linux architectures and tests each
# one in Docker containers before releasing.
#
# Usage:
#   ./tests/deb/build-and-test.sh
#
# Requirements:
#   - Docker must be installed and running
#   - .NET SDK must be installed
#   - nfpm must be installed (brew install nfpm or go install github.com/goreleaser/nfpm/v2/cmd/nfpm@latest)
#
# Exit codes:
#   0 - All builds and tests passed
#   1 - Build or test failed
#
# Notes:
#   - Only packages matching the host architecture are tested
#   - Cross-architecture testing requires QEMU emulation (slow)
#

set -eo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

cd "$REPO_ROOT"

# Detect host architecture
HOST_ARCH=$(uname -m)
case "$HOST_ARCH" in
    x86_64)  HOST_DEB_ARCH="amd64"; HOST_RID="linux-x64" ;;
    aarch64) HOST_DEB_ARCH="arm64"; HOST_RID="linux-arm64" ;;
    arm64)   HOST_DEB_ARCH="arm64"; HOST_RID="linux-arm64" ;;
    *) echo "Unknown host architecture: $HOST_ARCH"; exit 1 ;;
esac

# Get version from csproj
VERSION=$(./scripts/get-version.sh)
echo "Building and testing keystone-cli v$VERSION"
echo "Host architecture: $HOST_ARCH ($HOST_DEB_ARCH)"
echo "==========================================="

# Define build targets: RID|arch
BUILD_TARGETS=(
    "linux-x64|amd64"
    "linux-arm64|arm64"
)

# Test images to use
TEST_IMAGES="debian:bookworm ubuntu:24.04"

# Build all packages first
echo ""
echo "=== Building packages ==="

for target in "${BUILD_TARGETS[@]}"; do
    rid="${target%%|*}"
    echo ""
    echo "--- Building $rid ---"

    echo "Publishing..."
    dotnet publish ./src/Keystone.Cli/Keystone.Cli.csproj -c Release -r "$rid" -v quiet

    echo "Packaging .deb..."
    ./scripts/package-deb.sh "$VERSION" "$rid"
done

echo ""
echo "=== Built packages ==="
ls -la artifacts/release/*.deb

# Test packages
echo ""
echo "=== Testing packages ==="

FAILED=0
TESTED=0

for target in "${BUILD_TARGETS[@]}"; do
    rid="${target%%|*}"
    arch="${target##*|}"

    deb_file="artifacts/release/keystone-cli_${VERSION}_${arch}.deb"

    if [[ ! -f "$deb_file" ]]; then
        echo "ERROR: Package not found: $deb_file"
        FAILED=1
        continue
    fi

    # Check if we can test this architecture
    if [[ "$arch" != "$HOST_DEB_ARCH" ]]; then
        echo ""
        echo "--- Skipping $deb_file (requires $arch, host is $HOST_DEB_ARCH) ---"
        echo "    To test cross-architecture, use QEMU emulation."
        continue
    fi

    # Test on each image
    for image in $TEST_IMAGES; do
        echo ""
        echo "--- Testing $deb_file on $image ---"

        if ./tests/deb/test-package.sh "$deb_file" "$image"; then
            echo "PASSED: $rid on $image"
            ((TESTED++))
        else
            echo "FAILED: $rid on $image"
            FAILED=1
        fi
    done
done

echo ""
echo "==========================================="

if [[ $TESTED -eq 0 ]]; then
    echo "WARNING: No packages were tested."
    echo "Built packages (untested):"
    ls artifacts/release/*.deb
    exit 1
elif [[ $FAILED -eq 0 ]]; then
    echo "All tests passed! ($TESTED tests)"
    echo ""
    echo "Packages ready for release:"
    ls artifacts/release/*.deb
    exit 0
else
    echo "Some tests failed. Check output above."
    exit 1
fi
