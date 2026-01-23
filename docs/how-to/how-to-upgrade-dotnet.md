# How to Upgrade .NET Versions

This guide explains how to upgrade the project to a new .NET version (e.g., from .NET 10 to .NET 11).

## Overview

The project is designed to minimize hardcoded .NET version references. Both the SDK version and
target framework are defined in central locations, and packaging scripts dynamically extract
values at runtime. This architecture means most files automatically adapt when you update the
central configuration.

## Architecture

### SDK Version (global.json)

The .NET SDK version is defined in [`global.json`](../../global.json). This file is honored by:

- **Local builds** — The `dotnet` CLI searches upward for `global.json` and uses the specified SDK
- **CI builds** — The composite action at [`.github/actions/setup-dotnet`](
  ../../.github/actions/setup-dotnet/action.yml) reads from `global.json`

This ensures local and CI builds use the same SDK version.

### Target Framework (Directory.Build.props)

The target framework is defined once in [`Directory.Build.props`](../../Directory.Build.props).
All `.csproj` files inherit this value, so there's no need to update individual project files.

### Dynamic Path Resolution

Packaging scripts use [`scripts/get-tfm.sh`](../../scripts/get-tfm.sh) to extract the target
framework at runtime. This value constructs artifact paths like
`artifacts/bin/Keystone.Cli/Release/${TFM}/${RID}/publish`.

The following files use dynamic resolution and **do not need manual updates**:

- [`scripts/get-tfm.sh`](../../scripts/get-tfm.sh)
- [`scripts/package-release.sh`](../../scripts/package-release.sh)
- [`scripts/package-deb.sh`](../../scripts/package-deb.sh)
- [`nfpm.yaml`](../../nfpm.yaml)

## Upgrade Checklist

When upgrading to a new .NET version, update these files:

### 1. global.json

Update the `version` field in [`global.json`](../../global.json):

```json
{
    "sdk": {
        "version": "11.0.x",
        "rollForward": "latestFeature"
    }
}
```

### 2. Directory.Build.props

Update the `<TargetFramework>` element in [`Directory.Build.props`](../../Directory.Build.props):

```xml

<TargetFramework>net11.0</TargetFramework>
```

### 3. CLAUDE.md (Optional)

If the Framework and Dependencies section in [`CLAUDE.md`](../../CLAUDE.md) mentions a specific
.NET version, update it to match.

## Verification

After making the changes, verify the upgrade:

```bash
# Verify SDK version is detected
dotnet --version

# Clean previous build artifacts
rm -rf artifacts

# Restore and build
dotnet restore
dotnet build

# Run tests
dotnet test

# Verify the target framework is detected correctly
./scripts/get-tfm.sh

# Test publishing (optional)
dotnet publish src/Keystone.Cli -c Release -r osx-arm64
```

## Summary

| File                                      | Update Required | Notes                                 |
|-------------------------------------------|-----------------|---------------------------------------|
| `global.json`                             | Yes             | SDK version for local and CI builds   |
| `Directory.Build.props`                   | Yes             | Target framework (TFM)                |
| `CLAUDE.md`                               | Optional        | Documentation reference               |
| `.github/actions/setup-dotnet/action.yml` | No              | Reads from `global.json`              |
| `.github/workflows/*.yml`                 | No              | Uses composite action                 |
| `scripts/*.sh`                            | No              | Uses dynamic `${TFM}`                 |
| `nfpm.yaml`                               | No              | Uses dynamic `${TFM}`                 |
| `*.csproj`                                | No              | Inherits from `Directory.Build.props` |
