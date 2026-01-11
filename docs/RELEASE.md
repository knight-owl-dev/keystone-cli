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

## Release steps

1. Sync and validate locally:

   ```bash
   git checkout main
   git pull
   dotnet test ./tests/Keystone.Cli.UnitTests/Keystone.Cli.UnitTests.csproj -c Release
   ```

2. Ensure the `<Version>` value in `Keystone.Cli.csproj` is updated to match the intended release version.

   This version **must match** the git tag you are about to create (e.g., `<Version>0.1.1</Version>` → `v0.1.1`).

3. Create and push an annotated tag:

   ```bash
   git tag -a vX.Y.Z -m "keystone-cli vX.Y.Z"
   git push origin vX.Y.Z
   ```

   Example:

   ```bash
   git tag -a v0.1.0 -m "keystone-cli v0.1.0"
   git push origin v0.1.0
   ```

4. Monitor the GitHub Actions **Release** workflow.

   The workflow will:

    - `dotnet publish` for `osx-arm64` and `osx-x64`
    - run `scripts/package-release.sh` to build `.tar.gz` assets
    - compute SHA-256 values
    - create a GitHub Release for the tag and upload assets

5. Update the Homebrew formula in `Knight-Owl-Dev/homebrew-tap`:

    - Update the version and release URLs to `vX.Y.Z`
    - Replace the `sha256` values with the ones printed by the workflow
    - Commit and push the formula update

6. Validate installation on macOS:

   ```bash
   brew update
   brew install keystone-cli
   keystone-cli info
   man keystone-cli
   ```

---

## Notes

- The release assets must be publicly downloadable.
- GitHub **Release immutability** is enabled to prevent replacing assets after publishing.
- The packaging script includes:
    - `keystone-cli`
    - `appsettings.json`
    - `keystone-cli.1` (man page)
