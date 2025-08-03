# Keystone CLI

A command-line interface for Keystone.

This CLI is designed to operate alongside official Keystone templates, which define the structure and build behavior for
publishing books and documents. It is part of the broader Keystone ecosystem, which includes:

- [keystone-template-core](https://github.com/knight-owl-dev/keystone-template-core) – for building a full Docker image
  from scratch
- [keystone-template-core-slim](https://github.com/knight-owl-dev/keystone-template-core-slim) – for using a prebuilt
  public Docker image

For license details and third-party references, see [NOTICE.md](NOTICE.md).

## Project Structure

The project is organized into four main parts:

- [artifacts](artifacts) – Contains the generated artifacts, including binaries and documentation.
- [docs](docs) – Contains documentation files, including man pages in [mdoc(7)](https://man.openbsd.org/mdoc.7)
  format and other relevant documentation.
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
