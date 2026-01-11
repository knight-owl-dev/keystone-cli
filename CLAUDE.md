# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

```
keystone-cli/
├── src/
│   └── Keystone.Cli/           # Main application
│       ├── Application/        # Business logic layer
│       ├── Domain/             # Domain models and policies
│       ├── Presentation/       # Command controllers
│       └── Configuration/      # Dependency injection setup
├── tests/
│   └── Keystone.Cli.UnitTests/ # Unit tests mirroring src structure
├── docs/
│   └── man/                    # Manual pages in mdoc format
├── artifacts/                  # Build outputs
├── Directory.Build.props       # MSBuild configuration
└── keystone-cli.sln            # Visual Studio solution
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

- **Target Framework**: .NET 9.0
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
