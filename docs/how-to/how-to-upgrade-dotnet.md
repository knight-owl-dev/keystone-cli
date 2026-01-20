# How to Upgrade .NET Versions

This guide explains how to upgrade the project to a new .NET version (e.g., from .NET 10 to .NET 11).

## Overview

The project is designed to minimize hardcoded .NET version references. The target framework is
defined in a single location, and packaging scripts dynamically extract this value at runtime.
This architecture means most files automatically adapt when you update the central configuration.

## Architecture

### Single Source of Truth

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

### 1. Directory.Build.props

Update the `<TargetFramework>` element in
[`Directory.Build.props`](../../Directory.Build.props).

### 2. GitHub Workflows

Update the `dotnet-version` in the `setup-dotnet` action across three workflow files:

- [`.github/workflows/ci.yml`](../../.github/workflows/ci.yml) — Setup .NET step
- [`.github/workflows/release.yml`](../../.github/workflows/release.yml) — Setup .NET steps
  in both `validate` and `build-assets` jobs
- [`.github/workflows/tag-release.yml`](../../.github/workflows/tag-release.yml) — Setup .NET step

### 3. CLAUDE.md (Optional)

If the Framework and Dependencies section in [`CLAUDE.md`](../../CLAUDE.md) mentions a specific
.NET version, update it to match.

## Verification

After making the changes, verify the upgrade:

```bash
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

| File                                | Update Required | Notes                                 |
|-------------------------------------|-----------------|---------------------------------------|
| `Directory.Build.props`             | Yes             | Single source of truth for TFM        |
| `.github/workflows/ci.yml`          | Yes             | SDK version for CI                    |
| `.github/workflows/release.yml`     | Yes             | SDK version (2 locations)             |
| `.github/workflows/tag-release.yml` | Yes             | SDK version for tagging               |
| `CLAUDE.md`                         | Optional        | Documentation reference               |
| `scripts/*.sh`                      | No              | Uses dynamic `${TFM}`                 |
| `nfpm.yaml`                         | No              | Uses dynamic `${TFM}`                 |
| `*.csproj`                          | No              | Inherits from `Directory.Build.props` |
