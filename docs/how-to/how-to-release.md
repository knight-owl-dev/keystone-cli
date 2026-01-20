# How To Release

Keystone CLI uses **tag-driven releases**.

- Pull requests must have passing unit tests before merge.
- Releases are created **only** when a version tag (e.g., `v0.1.0`) is pushed.
- A GitHub Actions workflow builds and publishes release artifacts.

---

## Prerequisites

- You have push access to the [Knight-Owl-Dev/keystone-cli](https://github.com/knight-owl-dev/keystone-cli) repository.
- Unit tests are green on `main`.
- Homebrew tap repository [Knight-Owl-Dev/homebrew-tap](https://github.com/knight-owl-dev/homebrew-tap) is available for
  formula updates.

---

## Version management

Use the `/version` command in Claude Code to view or update the project version.

### View current version

```text
/version
```

This displays the current version from `Keystone.Cli.csproj`.

### Update to a new version

```text
/version X.Y.Z
```

This updates the version in all required files:

- `src/Keystone.Cli/Keystone.Cli.csproj` (all five version properties)
- `docs/man/man1/keystone-cli.1` (`.Dd` document date and VERSION section)
- `tests/Keystone.Cli.UnitTests/Application/Commands/Info/InfoCommandTests.cs` (version assertion)

The version must follow semantic versioning format (`X.Y.Z`).

---

## Release flows

Keystone CLI supports two release flows:

1. **Automated GitHub release (preferred)** — push-button, reproducible, and audited
2. **Manual release (backup)** — for emergencies or CI outages

---

## Automated release (via GitHub Actions)

This is the **recommended** way to publish a new version.

### Prerequisites

- `<Version>` in `Keystone.Cli.csproj` is updated to the intended release version
- All changes are merged into `main`
- Unit tests are passing

### Steps

1. Update the project version using `/version X.Y.Z` in Claude Code (see [Version management](#version-management)).

   This value **must match** the git tag that will be created (`vX.Y.Z`).

2. Push changes to `main` (via PR as usual).

3. Trigger the **Tag release** workflow in GitHub:

   - Go to **Actions → Tag release**
   - Click **Run workflow**

   The workflow will:

   - read `<Version>` from `Keystone.Cli.csproj`
   - run unit tests (release gate)
   - create and push the annotated tag `vX.Y.Z`

4. The tag push automatically triggers the **Release** workflow.

   That workflow will:

   - validate `<Version>` matches the tag
   - build and publish binaries (matrix-based by RID)
   - package `.tar.gz` release assets
   - build `.deb` packages for Linux architectures (amd64, arm64)
   - compute SHA-256 checksums for all assets
   - generate release notes
   - create a GitHub Release and upload all assets

5. (Optional) Update the Homebrew formula in `Knight-Owl-Dev/homebrew-tap`:

   - Update the version and release URLs to `vX.Y.Z`
   - Replace the `sha256` values using `checksums.txt` from the release
   - Commit and push (or merge the automated PR, if enabled)

6. Validate installation on macOS:

   ```bash
   brew update
   brew install keystone-cli
   keystone-cli info
   man keystone-cli
   ```

---

## Manual release (backup / emergency)

Use this flow **only** if GitHub Actions is unavailable or requires debugging.

### Steps

1. Sync and validate locally:

   ```bash
   git checkout main
   git pull
   dotnet test ./tests/Keystone.Cli.UnitTests/Keystone.Cli.UnitTests.csproj -c Release
   ```

2. Ensure the version matches the intended release (use `/version` to check, `/version X.Y.Z` to update).

3. Build and package release assets locally:

   ```bash
   dotnet publish ./src/Keystone.Cli/Keystone.Cli.csproj -c Release -r osx-arm64
   dotnet publish ./src/Keystone.Cli/Keystone.Cli.csproj -c Release -r osx-x64
   dotnet publish ./src/Keystone.Cli/Keystone.Cli.csproj -c Release -r linux-x64
   dotnet publish ./src/Keystone.Cli/Keystone.Cli.csproj -c Release -r linux-arm64

   ./scripts/package-release.sh X.Y.Z
   ./scripts/package-deb.sh X.Y.Z  # requires nfpm
   ```

4. Create and push the annotated tag:

   ```bash
   git tag -a vX.Y.Z -m "keystone-cli vX.Y.Z"
   git push origin vX.Y.Z
   ```

5. Create the GitHub Release manually if it didn't get created automatically:

   - Upload the generated `.tar.gz` files
   - Include `checksums.txt` in the release assets
   - Paste checksum contents into the release description

6. Update the Homebrew formula as usual.

---

## Debian packages

The release workflow automatically builds `.deb` packages for Linux architectures:

- `keystone-cli_X.Y.Z_amd64.deb` (linux-x64)
- `keystone-cli_X.Y.Z_arm64.deb` (linux-arm64)

These packages are built using [nfpm](https://nfpm.goreleaser.com/) via `scripts/package-deb.sh`
and uploaded to GitHub Releases alongside the tarballs. The package configuration is defined in
`nfpm.yaml` at the repository root.

To build `.deb` packages locally (requires nfpm):

```bash
# Install nfpm (choose one)
brew install nfpm                                            # macOS/Linux with Homebrew
go install github.com/goreleaser/nfpm/v2/cmd/nfpm@latest     # with Go

# Build for a specific RID
./scripts/package-deb.sh 0.1.0 linux-x64

# Or build all Linux architectures
./scripts/package-deb.sh 0.1.0
```

### Package contents

The `.deb` packages install:

- `/opt/keystone-cli/keystone-cli` — the main binary
- `/opt/keystone-cli/appsettings.json` — application configuration
- `/usr/local/share/man/man1/keystone-cli.1` — man page
- `/usr/share/doc/keystone-cli/copyright` — license file
- `/usr/local/bin/keystone-cli` — symlink to the binary

### Future: apt repository

See [issue #49](https://github.com/knight-owl-dev/keystone-cli/issues/49) for progress on hosting
these packages in an apt repository for easier installation on Debian/Ubuntu systems.
