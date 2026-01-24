# How to Test .deb Packages

This guide explains how to test `.deb` packages locally before releasing to the apt repository.

## Why Test Locally?

The release workflow includes automated `.deb` testing in CI, but testing locally allows you to:

- Catch issues before pushing
- Debug problems interactively
- Test on additional distributions
- Verify fixes without waiting for CI

## Prerequisites

- Docker installed and running
- .NET SDK installed
- nfpm installed:

  ```bash
  # macOS/Linux with Homebrew
  brew install nfpm

  # Or with Go
  go install github.com/goreleaser/nfpm/v2/cmd/nfpm@latest
  ```

## Quick Start: Build and Test All

Run the full build and test suite:

```bash
./tests/deb/build-and-test.sh
```

This script:

1. Builds `.deb` packages for `linux-x64` and `linux-arm64`
2. Tests each package on Debian and Ubuntu
3. Reports pass/fail status

## Testing a Single Package

To test a specific `.deb` file:

```bash
# Build for a specific architecture
dotnet publish ./src/Keystone.Cli/Keystone.Cli.csproj -c Release -r linux-x64
./scripts/package-deb.sh 0.1.9 linux-x64

# Test on default image (debian:bookworm-slim)
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_amd64.deb

# Test on a specific image
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_amd64.deb ubuntu:24.04
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_amd64.deb debian:bullseye
```

## What the Tests Verify

The test script verifies:

| Check | Purpose |
|-------|---------|
| Package installs | Dependencies are satisfied |
| `keystone-cli info` | Binary runs without crashes |
| `keystone-cli --version` | Version output works |
| `man keystone-cli` | Man page is installed correctly |

## Testing on Additional Distributions

To test on distributions not in the default suite:

```bash
# Debian variants
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_amd64.deb debian:bullseye
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_amd64.deb debian:trixie

# Ubuntu variants
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_amd64.deb ubuntu:22.04
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_amd64.deb ubuntu:24.04
```

## Architecture Considerations

The tests run in Docker containers matching your host architecture:

- On Apple Silicon (arm64): Tests run in `arm64` containers
- On Intel/AMD (x64): Tests run in `amd64` containers

To test cross-architecture packages, you need Docker with QEMU emulation:

```bash
# Enable QEMU for multi-arch (if not already enabled)
docker run --rm --privileged multiarch/qemu-user-static --reset -p yes

# Now you can test arm64 packages on x64 host (slow but works)
./tests/deb/test-package.sh artifacts/release/keystone-cli_0.1.9_arm64.deb debian:bookworm
```

## CI Integration

The release workflow (`release.yml`) includes a `test-deb` job that automatically tests packages
before publishing. This job:

- Runs after `build-assets`
- Tests on Debian bookworm and Ubuntu 24.04
- Blocks `publish-release` if tests fail

## Troubleshooting

### Package installs but crashes

Check for missing runtime dependencies. Common issues:

- **libicu missing**: Fixed by enabling `<InvariantGlobalization>true</InvariantGlobalization>`
  in the project file. See [how-to-handle-globalization.md](how-to-handle-globalization.md).
- **Other .NET dependencies**: Self-contained publish should include everything, but verify
  with `ldd /opt/keystone-cli/keystone-cli` inside the container.

### Test fails on specific distribution

Run the test interactively to debug:

```bash
docker run --rm -it -v "$(pwd)/artifacts/release:/deb:ro" debian:bookworm bash

# Inside container
apt-get update
apt-get install -y /deb/keystone-cli_0.1.9_amd64.deb
keystone-cli info
```

### Man page not found

Verify the man page is installed:

```bash
ls -la /usr/local/share/man/man1/keystone-cli.1
```

If missing, check `nfpm.yaml` contents mapping.
