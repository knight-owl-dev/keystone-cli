# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

```
keystone-cli/
├── src/
│   └── Keystone.Cli/               # Main application
│       ├── Application/            # Business logic layer
│       │   ├── Commands/           # Command handlers (Browse, Info, New, Project)
│       │   ├── Data/               # Repository pattern with Stores/
│       │   ├── FileSystem/         # File operations and copying services
│       │   ├── GitHub/             # GitHub repository and template handling
│       │   ├── Project/            # Project model management
│       │   └── Utility/            # Serialization (JSON, YAML, Env), Text parsing
│       ├── Domain/                 # Domain models and policies
│       │   ├── FileSystem/         # File system abstractions
│       │   ├── Helpers/            # Domain helper utilities
│       │   ├── Policies/           # Validation rules
│       │   └── Project/            # Project models
│       ├── Presentation/           # Command controllers
│       │   ├── ComponentModel/     # Data annotations and validation
│       │   └── Project/            # Project-specific controllers
│       └── Configuration/          # Dependency injection setup
├── tests/
│   └── Keystone.Cli.UnitTests/     # Unit tests mirroring src structure
├── docs/
│   ├── how-to/                     # Procedural guides
│   │   ├── how-to-release.md       # Release process documentation
│   │   ├── how-to-security.md      # Workflow and script security guide
│   │   ├── how-to-test-man-page.md # Man page testing guide
│   │   ├── how-to-upgrade-dotnet.md # .NET version upgrade guide
│   │   └── how-to-workflow.md      # Development workflow guide
│   └── man/                        # Manual pages in mdoc format
├── .github/
│   ├── actions/                    # Composite actions
│   │   └── setup-dotnet/           # Shared .NET setup
│   ├── ISSUE_TEMPLATE/             # Issue templates
│   ├── workflows/                  # GitHub Actions
│   │   ├── ci.yml                  # CI pipeline (tests on PR/push)
│   │   ├── release.yml             # Release build and publish
│   │   └── tag-release.yml         # Manual tag creation workflow
│   ├── dependabot.yml              # Dependency update automation
│   ├── pull_request_template.md    # PR template
│   └── release.yml                 # Release notes configuration
├── CONTRIBUTING.md                 # Contribution guidelines
├── scripts/                        # Build and utility scripts
│   ├── generate-checksums.sh       # SHA256 checksum generation for releases
│   ├── get-english-month-year.sh   # Locale-safe date for man page updates
│   ├── get-tfm.sh                  # Extract target framework from Directory.Build.props
│   ├── get-version.sh              # Extract version from csproj
│   ├── package-deb.sh              # Debian package script (requires nfpm)
│   ├── package-release.sh          # Tarball packaging script
│   ├── validate-arch.sh            # Validate Debian architecture
│   ├── validate-rid.sh             # Validate .NET runtime identifier
│   ├── validate-version.sh         # Validate semantic version format
│   └── verify-deb-install.sh       # Verify .deb package installation
├── nfpm.yaml                       # nfpm configuration for .deb packaging
├── artifacts/                      # Build outputs
├── global.json                     # .NET SDK version (used by local and CI builds)
├── Directory.Build.props           # MSBuild configuration (target framework, build settings)
└── keystone-cli.sln                # Visual Studio solution
```

## Development Commands

### Building and Running

```bash
# Build the project
dotnet build

# Run the CLI
dotnet run --project src/Keystone.Cli

# Build for specific runtime (self-contained single-file)
dotnet publish -c Release -r osx-arm64 --self-contained src/Keystone.Cli
```

### Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Keystone.Cli.UnitTests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Code Quality

The project uses strict code analysis:

- `TreatWarningsAsErrors` is enabled
- Extensive .editorconfig with C# and ReSharper rules
- Uses Microsoft.VisualStudio.Threading.Analyzers

### Manual Pages

To test the man page locally:

```bash
# Using mandoc (preferred)
mandoc docs/man/man1/keystone-cli.1

# Using man with absolute path
man -M "$(cd docs/man && pwd)" keystone-cli
```

## Architecture Overview

### Framework and Dependencies

- **Target Framework**: .NET 10.0
- **CLI Framework**: Cocona (command-line application framework)
- **Testing**: NUnit with NSubstitute for mocking
- **Configuration**: Microsoft.Extensions.Configuration with JSON and environment variables
- **HTTP Client**: Microsoft.Extensions.Http
- **YAML Processing**: YamlDotNet

### Project Structure

The project follows clean architecture principles with three main layers:

#### Domain Layer (`src/Keystone.Cli/Domain/`)

- Contains core business models and domain logic
- Key models: `InfoModel`, `TemplateTargetModel`, `CliCommandResults`
- Domain policies and validation rules
- File system abstractions and project models

#### Application Layer (`src/Keystone.Cli/Application/`)

- Business logic and use case implementations
- Service implementations for core functionality:
    - **Commands**: Browse, Info, New, Project commands
    - **FileSystem**: File operations and copying services
    - **GitHub**: GitHub repository and template handling
    - **Project**: Project model management and policy enforcement
    - **Data**: Repository pattern implementations with multiple store types
    - **Utility**: Serialization services (JSON, YAML, Environment files)

#### Presentation Layer (`src/Keystone.Cli/Presentation/`)

- Command controllers using Cocona framework
- Decoupled from Application layer—can swap CLI frameworks without affecting business logic
- User interface and command-line interaction
- Controllers: `BrowseCommandController`, `InfoCommandController`, `NewCommandController`,
  `SubCommandsHostController`

##### Cocona Conventions

Cocona exposes method parameters as CLI options by default. To keep help output clean:

- **Never** add `CancellationToken` as a command method parameter—it appears as
  `--cancellation-token` in help output
- Instead, inject `ICoconaAppContextAccessor` via constructor and access the token from context:

```csharp
public class MyCommand(ICoconaAppContextAccessor contextAccessor)
{
    public async Task<int> RunAsync()
    {
        var cancellationToken = contextAccessor.Current?.CancellationToken ?? CancellationToken.None;
        // use cancellationToken...
    }
}
```

This convention is enforced by `CoconaCommandMethodConventionsTests`.

### Key Components

#### Template System

The CLI works with Keystone templates from GitHub repositories:

- **core**: Full Docker image template
- **core-slim**: Prebuilt Docker image template
- **hello-world**: Sample project template

#### Project Model Stores

Multiple storage implementations for project configuration:

- `EnvFileProjectModelStore`: Environment file-based storage
- `PandocFileProjectModelStore`: Pandoc configuration storage
- `PublishFileProjectModelStore`: Publishing configuration storage
- `SyncFileProjectModelStore`: Synchronization configuration storage

#### Dependency Injection

All services are registered in `DependenciesInstaller.cs` using Microsoft.Extensions.DependencyInjection.

### Testing Structure

Tests mirror the source structure in `tests/Keystone.Cli.UnitTests/`:

- Organized by layer (Application, Configuration, Domain, Presentation)
- Uses NUnit testing framework with NSubstitute for mocking
- Tests cover service implementations and command logic

#### Cocona Test Utilities (`Presentation/Cocona/`)

Framework-specific test infrastructure isolated from general test utilities:

- `CoconaAppContextFactory` — Creates `CoconaAppContext` instances for testing commands
  that need `ICoconaAppContextAccessor`
- `CoconaCommandMethodConventionsTests` — Enforces conventions (e.g., no `CancellationToken`
  parameters in command methods)

### Configuration

Application settings are loaded from `appsettings.json` and `appsettings.{Environment}.json`.
The CLI searches for configuration files using a lookup chain (first match wins):

1. `KEYSTONE_CLI_CONFIG_DIR` environment variable (all platforms)
2. User config directory (all platforms): `%APPDATA%\keystone-cli\` on Windows,
   `~/.config/keystone-cli/` on Linux/macOS
3. `/etc/keystone-cli/` (FHS system-wide, Linux/macOS only)
4. `AppContext.BaseDirectory` (fallback, same directory as the executable)

This allows Linux package maintainers to place configuration in `/etc/keystone-cli/`
while users can override with their user config directory or the environment variable.

Template repository URLs are configurable via settings.

### Build Configuration

- Multi-platform support (Windows, macOS, Linux with various architectures)
- Self-contained single-file publishing enabled
- Deterministic builds for CI/CD
- Centralized build outputs in `artifacts/` directory
- Target framework defined centrally in `Directory.Build.props`; see
  `docs/how-to/how-to-upgrade-dotnet.md` for upgrade instructions

## CI/CD and Release Process

### GitHub Workflows

All workflows use a shared composite action (`.github/actions/setup-dotnet`) that reads the
SDK version from `global.json` for centralized version management.

- **ci.yml**: Runs unit tests on PRs and pushes to main
- **tag-release.yml**: Manual workflow to create a version tag from csproj version
- **release.yml**: Triggered by `v*.*.*` tags; validates version, builds multi-platform
  binaries, packages tarballs and .deb files, generates checksums, and publishes GitHub Release

### Release Flow

1. Update version using `/version X.Y.Z` command (updates csproj, man page, and tests)
2. Create PR and merge to main
3. Run `tag-release.yml` workflow manually (creates and pushes tag)
4. `release.yml` triggers automatically on tag push
5. Release published with binaries for osx-arm64, osx-x64, linux-x64, linux-arm64, etc.

### GitHub Configuration

- **dependabot.yml**: Automated dependency updates for NuGet packages and GitHub Actions;
  runs weekly on Mondays with minor/patch updates grouped to reduce PR noise
- **release.yml** (in `.github/`): Configures auto-generated release notes categories
  (Breaking Changes, Security, Enhancements, Bug Fixes, Documentation, Dependencies)
- **pull_request_template.md**: PR template enforcing outcome-focused descriptions and
  label requirements

### Scripts

- **generate-checksums.sh**: Generates SHA256 checksums for release artifacts.
  Usage: `./scripts/generate-checksums.sh [dist-dir]`
- **get-english-month-year.sh**: Returns locale-safe English month and year for man page updates.
  Usage: `./scripts/get-english-month-year.sh`
- **get-tfm.sh**: Extracts the target framework moniker from `Directory.Build.props`.
  Usage: `./scripts/get-tfm.sh`
- **get-version.sh**: Extracts the project version from `Keystone.Cli.csproj`.
  Usage: `./scripts/get-version.sh`
- **package-deb.sh**: Creates `.deb` packages for Linux (amd64, arm64) using nfpm;
  requires `nfpm` (`brew install nfpm` or `go install github.com/goreleaser/nfpm/v2/cmd/nfpm@latest`).
  Usage: `./scripts/package-deb.sh [version] [rid]`
- **package-release.sh**: Creates release tarballs with binary, config, and man page.
  Usage: `./scripts/package-release.sh [version] [rid]`
- **validate-arch.sh**: Validates Debian architecture (amd64, arm64) for safe use in scripts.
  Usage: `./scripts/validate-arch.sh <arch>`
- **validate-rid.sh**: Validates .NET runtime identifier for safe use in scripts.
  Use `--linux` to restrict to Linux RIDs only (for .deb packaging).
  Usage: `./scripts/validate-rid.sh [--linux] <rid>`
- **validate-version.sh**: Validates semantic version format for safe use in filenames.
  Usage: `./scripts/validate-version.sh <version>`
- **verify-deb-install.sh**: Verifies .deb package installation inside a container;
  used by CI workflows and `tests/deb/test-package.sh`.
  Usage: `./scripts/verify-deb-install.sh <path-to-deb>`

For security best practices when modifying workflows or scripts, see
[docs/how-to/how-to-security.md](docs/how-to/how-to-security.md).
