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
│   │   ├── how-to-test-man-page.md # Man page testing guide
│   │   └── how-to-workflow.md      # Development workflow guide
│   └── man/                        # Manual pages in mdoc format
├── .github/
│   ├── ISSUE_TEMPLATE/             # Issue templates
│   ├── workflows/                  # GitHub Actions
│   │   ├── ci.yml                  # CI pipeline (tests on PR/push)
│   │   ├── release.yml             # Release build and publish
│   │   └── tag-release.yml         # Manual tag creation workflow
│   ├── pull_request_template.md    # PR template
│   └── release.yml                 # Release notes configuration
├── CONTRIBUTING.md                 # Contribution guidelines
├── scripts/                        # Build and utility scripts
│   └── package-release.sh          # Tarball packaging script
├── artifacts/                      # Build outputs
├── Directory.Build.props           # MSBuild configuration
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
- User interface and command-line interaction
- Controllers: `BrowseCommandController`, `InfoCommandController`, `NewCommandController`, `SubCommandsHostController`

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

### Configuration

- Application settings in `appsettings.json` and `appsettings.Development.json`
- Environment-specific configuration support
- Template repository URLs configurable via settings

### Build Configuration

- Multi-platform support (Windows, macOS, Linux with various architectures)
- Self-contained single-file publishing enabled
- Deterministic builds for CI/CD
- Centralized build outputs in `artifacts/` directory

## CI/CD and Release Process

### GitHub Workflows

- **ci.yml**: Runs unit tests on PRs and pushes to main
- **tag-release.yml**: Manual workflow to create a version tag from csproj version
- **release.yml**: Triggered by `v*.*.*` tags; validates version, builds multi-platform
  binaries, generates checksums, and publishes GitHub Release

### Release Flow

1. Update version using `/version X.Y.Z` command (updates csproj, man page, and tests)
2. Create PR and merge to main
3. Run `tag-release.yml` workflow manually (creates and pushes tag)
4. `release.yml` triggers automatically on tag push
5. Release published with binaries for osx-arm64, osx-x64, linux-x64, linux-arm64, etc.

### GitHub Configuration

- **release.yml** (in `.github/`): Configures auto-generated release notes categories
  (Breaking Changes, Security, Enhancements, Bug Fixes, Documentation, Dependencies)
- **pull_request_template.md**: PR template enforcing outcome-focused descriptions and
  label requirements

### Scripts

- **package-release.sh**: Creates release tarballs with binary, config, and man page;
  generates SHA256 checksums. Usage: `./scripts/package-release.sh [version] [rid]`
