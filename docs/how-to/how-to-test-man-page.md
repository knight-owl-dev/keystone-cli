# Testing the Man Page

All examples assume you're in the root directory of the project.

Man pages use the [mdoc(7)](https://man.openbsd.org/mdoc.7) format -- a semantic markup language designed specifically
for writing Unix manual pages. While the `man` command can be used to view them, the `mandoc` utility is preferred for
local testing and rendering.

To read the [mdoc(7)](https://man.openbsd.org/mdoc.7) documentation in your terminal:

```bash
man 7 mdoc
```

## Using `mandoc`

To test the generated man page locally:

```bash
mandoc docs/man/man1/keystone-cli.1
```

## Using `man`

To view the man page with man, you must supply an absolute path for the man path:

```bash
man -M "$(cd docs/man && pwd)" keystone-cli
```
