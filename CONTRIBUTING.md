# Contributing to Keystone CLI

Thank you for your interest in contributing to Keystone CLI. This document provides guidelines
and pointers to help you get started.

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Review the [README](README.md) for project overview and installation instructions
4. Review [CLAUDE.md](CLAUDE.md) for development commands and architecture details

## Development Setup

The project uses .NET 10.0 and follows clean architecture principles.

```bash
# Build the project
dotnet build

# Run tests
dotnet test

# Run the CLI
dotnet run --project src/Keystone.Cli
```

For complete build commands, testing options, and architecture details, see [CLAUDE.md](CLAUDE.md).

## Reporting Issues

Before opening an issue, search existing issues to avoid duplicates.

### Bug Reports

Use the [bug report template](.github/ISSUE_TEMPLATE/bug.md) for issues where something is not
working as expected. Include:

- What happened vs. what was expected
- Steps to reproduce
- Environment details (OS, CLI version)

### Feature Requests and Tasks

Use the [task template](.github/ISSUE_TEMPLATE/task.md) for enhancements, improvements, or
planned work. Focus on outcomes rather than implementation details.

### Using Claude Code

If you use [Claude Code](https://claude.ai/code), you can create issues and pull requests
directly from the terminal.

**Creating issues** with `/issue-create`:

- Prompts for a description (or accepts one as an argument)
- Determines the appropriate template (bug or task)
- Suggests labels based on the content
- Previews the issue before creation

**Creating pull requests** with `/pr-create`:

- Extracts the issue number from the branch name (e.g., `42-fix-login-bug`)
- Fetches the related issue to understand scope and context
- Analyzes commits to compose the Changes section
- Generates a PR following the project template
- Suggests labels based on the related issue

Both commands require the [GitHub CLI](https://cli.github.com/) to be installed and authenticated.

## Submitting Changes

This project follows a structured workflow documented in
[docs/how-to/how-to-workflow.md](docs/how-to/how-to-workflow.md).

### Quick Reference

1. Create a branch for your work
2. Make your changes with clear, focused commits
3. Ensure tests pass: `dotnet test`
4. Open a pull request using the [PR template](.github/pull_request_template.md)
5. Use an outcome-focused PR title (it becomes a release note entry)
6. Reference related issues with `Fixes #N` or `Refs #N`
7. Apply appropriate labels (`bug`, `enhancement`, `documentation`)

### Code Quality

- The project uses `TreatWarningsAsErrors` — all warnings must be resolved
- Follow existing code patterns and architecture
- Include unit tests for new functionality
- Shell scripts must pass `shfmt` formatting and `shellcheck` analysis
- GitHub Actions workflows must pass `actionlint` validation
  - Run `make lint` to check all locally
  - Run `make lint-fix` to auto-fix shell formatting
- Run `dotnet test` before submitting

## Labels

Apply labels to issues and PRs. Labels determine how changes are grouped in auto-generated
release notes. See [.github/release.yml](.github/release.yml) for the full categorization
configuration.

| Label              | Use For                                     |
|--------------------|---------------------------------------------|
| `breaking-change`  | Incompatible API or behavior change         |
| `security`         | Security fixes or improvements              |
| `enhancement`      | New feature or improvement                  |
| `bug`              | Something is broken                         |
| `documentation`    | Documentation changes                       |
| `dependencies`     | Dependency updates                          |
| `question`         | Clarification or discussion                 |
| `good-first-issue` | Good for newcomers                          |
| `help-wanted`      | Extra attention needed                      |
| `duplicate`        | Duplicate issue (excluded from notes)       |
| `invalid`          | Not valid (excluded from notes)             |
| `wont-fix`         | Will not be addressed (excluded from notes) |

## Release Process

Releases are tag-driven and automated. For maintainers, the complete release process is
documented in [docs/how-to/how-to-release.md](docs/how-to/how-to-release.md).

## Additional Resources

- [Development workflow](docs/how-to/how-to-workflow.md) — issue tracking, PRs, and merging
- [Release process](docs/how-to/how-to-release.md) — version management and publishing
- [Testing man pages](docs/how-to/how-to-test-man-page.md) — local man page validation
- [Security best practices](docs/how-to/how-to-security.md) — GitHub Actions and shell script security
- [Architecture and commands](CLAUDE.md) — project structure and development reference

## Questions

For questions or discussion, open an issue with the `question` label or start a
[GitHub Discussion](https://github.com/knight-owl-dev/keystone-cli/discussions) if enabled.
