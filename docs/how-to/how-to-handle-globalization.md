# How to Handle Globalization

This guide documents the globalization approach for `keystone-cli` and considerations for future changes.

## Current Approach: Invariant Mode

Keystone-cli uses .NET's [globalization invariant mode][invariant-mode], configured in `Keystone.Cli.csproj`:

```xml
<InvariantGlobalization>true</InvariantGlobalization>
```

This eliminates the `libicu` runtime dependency, allowing the `.deb` package to work on any
Debian/Ubuntu version without version-specific dependencies.

### Why Invariant Mode?

The alternative—declaring `libicu` as a package dependency—has significant drawbacks:

1. **Version fragmentation**: Each Debian/Ubuntu release ships a different libicu version:
    - Debian 11 (Bullseye): libicu67
    - Debian 12 (Bookworm): libicu72
    - Debian 13 (Trixie): libicu76
    - Ubuntu 22.04: libicu70
    - Ubuntu 24.04: libicu74

2. **OR dependencies require maintenance**: You can use pipe syntax (`libicu74 | libicu72 | ...`)
   but must update the list for each new distro release.

3. **Breaking changes**: Microsoft's .NET packages have repeatedly broken on new distro releases
   due to `libicu` version mismatches. See [dotnet/sdk#48973][sdk-issue] and
   [PowerShell/PowerShell#25865][pwsh-issue].

### Behavior in Invariant Mode

With invariant mode enabled:

- String comparisons use ordinal (byte-by-byte) comparison, not linguistic rules
- Culture-specific formatting uses the invariant culture
- `CultureInfo` creation for non-invariant cultures throws `CultureNotFoundException`
- Internationalized Domain Names (IDN) don't conform to the latest standards

For a CLI tool like keystone-cli, these limitations are acceptable. The tool doesn't perform
locale-sensitive string sorting or culture-specific formatting.

## Adding Full Globalization Support

If future features require full globalization (e.g., locale-aware sorting, culture-specific
date/number formatting), follow these steps:

### 1. Remove Invariant Mode

In `Keystone.Cli.csproj`, remove or set to false:

```xml
<InvariantGlobalization>false</InvariantGlobalization>
```

### 2. Add libicu Dependencies

In `nfpm.yaml`, add OR dependencies covering target distributions:

```yaml
depends:
  - libicu74 | libicu72 | libicu71 | libicu70 | libicu67
```

**Important**: This list must be updated when targeting new distributions. Check the `libicu` version
in each target distro:

```bash
# On the target system
apt-cache show libicu[0-9]* 2>/dev/null | grep -E "^Package:" | head -1
```

### 3. Update CI Testing

Add matrix testing across target distributions to catch dependency issues early:

```yaml
strategy:
  matrix:
    image:
      - debian:bullseye
      - debian:bookworm
      - ubuntu:22.04
      - ubuntu:24.04
```

### 4. Document Supported Distributions

Maintain a list of tested distributions and their `libicu` versions. When a new distro releases
with a new `libicu` version, update the `depends` list and test.

## Alternative: App-Local ICU

.NET supports bundling ICU data with the application via [app-local ICU][app-local-icu]. This
increases binary size (~30MB) but eliminates system dependency issues:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.ICU.ICU4C.Runtime" Version="72.1.0.3" />
  <RuntimeHostConfigurationOption Include="System.Globalization.AppLocalIcu"
                                   Value="72.1.0.3" />
</ItemGroup>
```

This approach trades binary size for deployment simplicity.

## References

- [.NET Globalization Invariant Mode][invariant-mode] — Official documentation
- [Globalization Invariant Mode Design][invariant-design] — Design rationale and behavior details
- [dotnet/sdk#48973][sdk-issue] — `libicu76` compatibility issue on Debian Trixie
- [PowerShell/PowerShell#25865][pwsh-issue] — Similar issue affecting PowerShell packages
- [App-local ICU][app-local-icu] — Bundling ICU with the application

[invariant-mode]: https://learn.microsoft.com/en-us/dotnet/core/runtime-config/globalization

[invariant-design]: https://github.com/dotnet/runtime/blob/main/docs/design/features/globalization-invariant-mode.md

[sdk-issue]: https://github.com/dotnet/sdk/issues/48973

[pwsh-issue]: https://github.com/PowerShell/PowerShell/issues/25865

[app-local-icu]: https://learn.microsoft.com/en-us/dotnet/core/extensions/globalization-icu#app-local-icu
