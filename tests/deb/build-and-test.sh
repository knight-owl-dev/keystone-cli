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

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

cd "$REPO_ROOT"

# Get version from csproj
VERSION=$(./scripts/get-version.sh)
echo "Building and testing keystone-cli v$VERSION"
echo "==========================================="

# Define architectures and their test images
declare -A ARCH_IMAGES=(
    ["linux-x64"]="debian:bookworm ubuntu:24.04"
    ["linux-arm64"]="debian:bookworm ubuntu:24.04"
)

# Build all packages first
echo ""
echo "=== Building packages ==="

for rid in "${!ARCH_IMAGES[@]}"; do
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

# Test each package
echo ""
echo "=== Testing packages ==="

FAILED=0

for rid in "${!ARCH_IMAGES[@]}"; do
    # Map RID to Debian architecture
    case "$rid" in
        linux-x64)  arch="amd64" ;;
        linux-arm64) arch="arm64" ;;
        *) echo "Unknown RID: $rid"; exit 1 ;;
    esac

    deb_file="artifacts/release/keystone-cli_${VERSION}_${arch}.deb"

    if [[ ! -f "$deb_file" ]]; then
        echo "ERROR: Package not found: $deb_file"
        FAILED=1
        continue
    fi

    # Test on each image for this architecture
    for image in ${ARCH_IMAGES[$rid]}; do
        echo ""
        echo "--- Testing $deb_file on $image ---"

        if ./tests/deb/test-package.sh "$deb_file" "$image"; then
            echo "PASSED: $rid on $image"
        else
            echo "FAILED: $rid on $image"
            FAILED=1
        fi
    done
done

echo ""
echo "==========================================="

if [[ $FAILED -eq 0 ]]; then
    echo "All tests passed!"
    echo ""
    echo "Packages ready for release:"
    ls artifacts/release/*.deb
    exit 0
else
    echo "Some tests failed. Check output above."
    exit 1
fi
