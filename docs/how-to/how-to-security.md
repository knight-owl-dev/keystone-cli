# How to Secure Workflows and Scripts

This guide documents security best practices for GitHub Actions workflows and shell scripts
in this repository. These patterns were established in
[PR #112](https://github.com/knight-owl-dev/keystone-cli/pull/112) to prevent script injection
and ensure safe handling of user-controlled inputs.

## GitHub Actions Security

### Script Injection Prevention

**Problem**: Direct interpolation of GitHub expressions (`${{ }}`) in shell blocks can lead
to script injection if the value contains shell metacharacters.

**Solution**: Pass expressions through environment variables using `env:` blocks.

| Do                      | Don't                     |
|-------------------------|---------------------------|
| `env:` block + `"$VAR"` | Direct `${{ }}` in `run:` |

**Example - Safe pattern** (from `.github/workflows/release.yml`):

```yaml
- name: Validate csproj version matches tag
  shell: bash
  env:
    TAG_VERSION: ${{ steps.v.outputs.version }}
  run: |
    CS_VERSION="$(./scripts/get-version.sh)"
    if [[ "$CS_VERSION" != "$TAG_VERSION" ]]; then
      echo "ERROR: csproj <Version> ($CS_VERSION) does not match tag (v$TAG_VERSION)" >&2
      exit 1
    fi
```

**Example - Unsafe pattern** (avoid):

```yaml
# UNSAFE: Direct interpolation allows injection
- name: Validate version
  run: |
    if [[ "$(./scripts/get-version.sh)" != "${{ steps.v.outputs.version }}" ]]; then
      exit 1
    fi
```

### Least-Privilege Permissions

**Principle**: Explicitly declare permissions at both workflow and job levels.
Only request the minimum permissions needed for each job.

| Workflow/Job                | Permissions           | Purpose                          |
|-----------------------------|-----------------------|----------------------------------|
| ci.yml (workflow)           | `contents: read`      | Read repository for testing      |
| ci.yml changes job          | `pull-requests: read` | Read PR for path filtering       |
| release.yml validate        | `contents: read`      | Read repository for validation   |
| release.yml build-assets    | `contents: read`      | Read repository for building     |
| release.yml publish-release | `contents: write`     | Create release and upload assets |

**Example** (from `.github/workflows/ci.yml`):

```yaml
permissions:
  contents: read

jobs:
  changes:
    runs-on: ubuntu-latest
    permissions:
      pull-requests: read
```

### Input Validation in Composite Actions

Validate all inputs before using them in shell commands. Use dedicated validation scripts
that return sanitized output or exit with an error.

**Example** (from `.github/actions/build-deb/action.yml`):

```yaml
- name: Validate RID
  shell: bash
  env:
    RID: ${{ inputs.rid }}
  run: ./scripts/validate-rid.sh --linux "$RID" > /dev/null

- name: Validate version
  if: inputs.version != ''
  shell: bash
  env:
    VERSION: ${{ inputs.version }}
  run: ./scripts/validate-version.sh "$VERSION" > /dev/null
```

## Shell Script Security

### Strict Mode

All scripts should start with strict mode to catch errors early:

```bash
set -euo pipefail
```

| Flag          | Meaning                  | Benefit                          |
|---------------|--------------------------|----------------------------------|
| `-e`          | Exit on error            | Prevents silent failures         |
| `-u`          | Error on unset variables | Catches typos and missing inputs |
| `-o pipefail` | Propagate pipe errors    | Catches failures in pipelines    |

### Automated Analysis

ShellCheck is integrated into CI to catch common issues automatically. Run `make lint`
locally before committing. The `.shellcheckrc` configuration enables stricter checks
including `require-variable-braces`, `quote-safe-variables`, and `deprecate-which`.

### Input Validation with Allowlists

Validate all external inputs against explicit allowlists. The pattern is:
validate, echo the sanitized value, or exit with an error.

| Script                | Validates               | Allowlist                                                          |
|-----------------------|-------------------------|--------------------------------------------------------------------|
| `validate-version.sh` | Semantic version        | Regex: `^[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*)?$` |
| `validate-rid.sh`     | .NET runtime identifier | `osx-arm64`, `osx-x64`, `linux-x64`, `linux-arm64`                 |
| `validate-arch.sh`    | Debian architecture     | `amd64`, `arm64`                                                   |

**Example pattern** (from `scripts/validate-arch.sh`):

```bash
#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Usage: $(basename "$0") <arch>" >&2
    exit 1
fi

ARCH="$1"

case "$ARCH" in
    amd64|arm64)
        echo "$ARCH"
        ;;
    *)
        echo "ERROR: Invalid architecture: $ARCH" >&2
        echo "Supported architectures: amd64, arm64" >&2
        exit 1
        ;;
esac
```

**Usage in workflows** (from `.github/workflows/ci.yml`):

```bash
SAFE_ARCH="$(./scripts/validate-arch.sh "$ARCH")"
bash ./scripts/verify-deb-install.sh ./deb/keystone-cli_*_"$SAFE_ARCH".deb
```

### Safe Variable Quoting

**Rule**: Always quote variables in shell commands to prevent word splitting and globbing.

| Do           | Don't      |
|--------------|------------|
| `"$VERSION"` | `$VERSION` |
| `"${RID}"`   | `${RID}`   |

**Exception**: When intentional globbing is needed, leave the glob unquoted but add a
comment explaining why.

**Example** (from `.github/workflows/ci.yml`):

```bash
# SAFE_ARCH is the validated architecture;
# * is left unquoted for globbing and "$SAFE_ARCH" is quoted after the glob.
bash ./scripts/verify-deb-install.sh ./deb/keystone-cli_*_"$SAFE_ARCH".deb
```

### Defense in Depth

Even when upstream validation exists, add local guards for critical operations.
This protects against refactoring that might remove upstream checks.

**Example** (from `scripts/package-deb.sh`):

```bash
# Validate RID (--linux restricts to linux-x64, linux-arm64)
RID="$("${SCRIPT_DIR}/validate-rid.sh" --linux "$RID")"

# Map RID to Debian architecture (validate-rid.sh --linux restricts values,
# but we guard against unexpected RIDs for safety)
case "$RID" in
  linux-x64)   ARCH="amd64" ;;
  linux-arm64) ARCH="arm64" ;;
  *)
    echo "ERROR: Unexpected RID: $RID" >&2
    exit 1
    ;;
esac
```

### Unsafe Constructs to Avoid

| Construct         | Problem                  | Alternative                       |
|-------------------|--------------------------|-----------------------------------|
| `eval "$var"`     | Arbitrary code execution | Use case statements or allowlists |
| `$var` (unquoted) | Word splitting, globbing | Always quote: `"$var"`            |
| `` `command` ``   | Harder to read, nest     | Use `$(command)`                  |
| `echo $var`       | Expansion issues         | Use `printf '%s\n' "$var"`        |

## Quick Reference

| Pattern              | Example Location                             | Description                    |
|----------------------|----------------------------------------------|--------------------------------|
| `env:` blocks        | `.github/workflows/release.yml:35-36`        | Safe GitHub expression passing |
| Explicit permissions | `.github/workflows/ci.yml:9-10`              | Least-privilege access         |
| Input validation     | `.github/actions/build-deb/action.yml:29-40` | Validate before use            |
| `set -euo pipefail`  | `scripts/validate-version.sh:16`             | Strict mode                    |
| Allowlist validation | `scripts/validate-arch.sh:24-33`             | Safe input handling            |
| Quoted variables     | `scripts/package-deb.sh:83`                  | Prevent word splitting         |
| Defense in depth     | `scripts/package-deb.sh:104-113`             | Redundant safety checks        |

## External Resources

- [GitHub Actions Security Hardening](https://docs.github.com/en/actions/security-for-github-actions/security-guides/security-hardening-for-github-actions)
- [Keeping your GitHub Actions and workflows secure](https://securitylab.github.com/resources/github-actions-preventing-pwn-requests/)
- [ShellCheck](https://www.shellcheck.net/) - Static analysis for shell scripts

## See Also

- [CLAUDE.md](../../CLAUDE.md) - Development reference and architecture
- [CONTRIBUTING.md](../../CONTRIBUTING.md) - Contribution guidelines
- [PR #112](https://github.com/knight-owl-dev/keystone-cli/pull/112) - Security hardening changes
