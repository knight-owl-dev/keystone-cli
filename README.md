# Keystone CLI

[![Homebrew](https://img.shields.io/badge/install-homebrew-brightgreen)](https://brew.sh)

A command-line interface for Keystone.

- 📦 [Releases](https://github.com/Knight-Owl-Dev/keystone-cli/releases) — binaries & checksums
- 📄 [License & Notices](NOTICE.md)

This CLI is designed to operate alongside official Keystone templates, which define the structure and build behavior for
publishing books and documents. It is part of the broader Keystone ecosystem, which includes:

- [keystone-template-core](https://github.com/knight-owl-dev/keystone-template-core) – for building a full Docker image
  from scratch
- [keystone-template-core-slim](https://github.com/knight-owl-dev/keystone-template-core-slim) – for using a prebuilt
  public Docker image
- [keystone-hello-world](https://github.com/knight-owl-dev/keystone-hello-world) – a "Hello World" sample project
  based on the `core-slim` template, demonstrating Keystone capabilities with sample content

## Installation

Keystone CLI is distributed via Homebrew.

First, add the Knight Owl Homebrew tap:

```bash
brew tap knight-owl-dev/tap
```

Then install the CLI:

```bash
brew install keystone-cli
```

After installation, verify that everything is working:

```bash
keystone-cli info
man keystone-cli
```

## Project Structure

The project is organized into four main parts:

- [docs](docs) – Contains documentation files, including man pages in [mdoc(7)](https://man.openbsd.org/mdoc.7)
  format and other relevant documentation.
- [scripts](scripts) – Build and utility scripts.
- [src](src) – Application source code.
- [tests](tests) – Unit tests organized to mirror the structure of the application for consistency and
  coverage.

## Testing the Man Page

All examples assume you're in the root directory of the project.

Man pages use the [mdoc(7)](https://man.openbsd.org/mdoc.7) format -- a semantic markup language designed specifically
for writing Unix manual pages. While the `man` command can be used to view them, the `mandoc` utility is preferred for
local testing and rendering.

To read the [mdoc(7)](https://man.openbsd.org/mdoc.7) documentation in your terminal:

```bash
man 7 mdoc
```

### Using `mandoc`

To test the generated man page locally:

```bash
mandoc docs/man/man1/keystone-cli.1
```

### Using `man`

To view the man page with man, you must supply an absolute path for the man path:

```bash
man -M "$(cd docs/man && pwd)" keystone-cli
```
