# Keystone CLI

[![Homebrew](https://img.shields.io/badge/install-homebrew-brightgreen)](https://brew.sh)
[![Apt](https://img.shields.io/badge/install-apt-blue)](https://apt.knight-owl.dev)

Command-line interface for Keystone.

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

### Homebrew (macOS/Linux)

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

### Apt (Debian/Ubuntu)

First, import the GPG signing key and add the repository:

```bash
curl -fsSL https://apt.knight-owl.dev/PUBLIC.KEY | sudo gpg --dearmor -o /usr/share/keyrings/knight-owl.gpg
echo "deb [signed-by=/usr/share/keyrings/knight-owl.gpg] https://apt.knight-owl.dev stable main" | sudo tee /etc/apt/sources.list.d/knight-owl.list
```

Then install the CLI:

```bash
sudo apt-get update
sudo apt-get install keystone-cli
```

After installation, verify that everything is working:

```bash
keystone-cli info
man keystone-cli
```

## Project Structure

The project is organized into four main parts:

- [docs](docs) – Contains how-tos, documentation files, including man pages in [mdoc(7)](https://man.openbsd.org/mdoc.7)
  format and other relevant documentation.
- [scripts](scripts) – Build and utility scripts.
- [src](src) – Application source code.
- [tests](tests) – Unit tests organized to mirror the structure of the application for consistency and
  coverage.

Review the contents of [docs/how-to](docs/how-to) for detailed guides on working with the project, contributing,
and releasing new versions.
