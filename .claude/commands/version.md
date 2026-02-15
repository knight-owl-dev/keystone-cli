# Version Management Command

Manage the project version across all configuration files.

## Arguments: $ARGUMENTS

## Instructions

1. Read the current version by running the version script:

```bash
./scripts/get-version.sh
```

2. **If arguments are empty or "show"**: Display the current version and exit

3. **If a version is provided**: Validate and update the version

### Validation

The new version MUST match the pattern `X.Y.Z` where X, Y, and Z are non-negative integers
(e.g., `0.1.0`, `1.0.0`, `2.3.14`). If the pattern doesn't match, show an error and exit.

### Files to Update

When bumping the version, update these files:

#### 1. `src/Keystone.Cli/Keystone.Cli.csproj`

Update ALL five version properties to the new value:

- `<Version>X.Y.Z</Version>`
- `<ApplicationVersion>X.Y.Z</ApplicationVersion>`
- `<AssemblyVersion>X.Y.Z</AssemblyVersion>`
- `<FileVersion>X.Y.Z</FileVersion>`
- `<InformationalVersion>X.Y.Z</InformationalVersion>`

#### 2. `docs/man/man1/keystone-cli.1`

Update TWO sections in this file:

**a) The `.Dd` date tag (line 1):**

Run the locale-safe date script to get the current month and year:

```bash
./scripts/get-english-month-year.sh
```

Update the first line of the man page to `.Dd <output>` (e.g., `.Dd January 1, 2026`).

**b) The VERSION section:**

Find the line that starts with `keystone-cli` under the `.Sh VERSION` section and
update it to `keystone-cli X.Y.Z`.

#### 3. `tests/Keystone.Cli.UnitTests/Application/Commands/Info/InfoCommandTests.cs`

Update the version assertion. Find the line containing `Does.StartWith("X.Y.Z")` and update
it to use the new version.

### Verify

After updating all files, run the unit tests to ensure everything still passes:

```bash
dotnet test
```

### Output

After updating and verifying, confirm the changes by showing:

- The old version
- The new version
- List of files updated
- Test results (passed/failed)
