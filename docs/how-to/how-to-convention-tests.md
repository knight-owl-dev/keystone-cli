# Convention Tests

This document explains how we use unit tests to enforce coding conventions and catch violations early.

## Why Convention Tests?

Static analyzers and linters catch many issues, but some conventions are project-specific or framework-specific
and can't be enforced with off-the-shelf tooling. Convention tests fill this gap by using reflection to
inspect the codebase at test time and flag violations.

Benefits:

- **Automated enforcement** – Violations fail CI, preventing merge
- **Self-documenting** – The test code explains the convention and the fix
- **Low maintenance** – No need to configure external tools or write custom analyzers
- **Immediate feedback** – Developers see violations locally before pushing

## Cocona Command Conventions

The CLI uses [Cocona](https://github.com/mayuki/Cocona) as its command-line framework. Cocona exposes
method parameters as CLI options by default, which can lead to unintended behavior if conventions aren't
followed.

Convention tests live in:

```text
tests/Keystone.Cli.UnitTests/Presentation/Cocona/CoconaCommandMethodConventionsTests.cs
```

### No CancellationToken Parameters

**Convention:** Command methods must not accept `CancellationToken` as a parameter.

**Why:** Cocona exposes all parameters as CLI options. A `CancellationToken` parameter would appear
as `--cancellation-token` in help output, confusing users.

**Fix:** Inject `ICoconaAppContextAccessor` via constructor and access the token from context:

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

### Short Aliases Required

**Convention:** All `[Option]` parameters must have a short alias character.

**Why:** Short aliases (`-t`, `-p`, `-g`) follow Unix conventions, improve discoverability, and make
CLI invocations more concise. Consistency across all options helps users build muscle memory.

**Fix:** Add a short alias character to the `OptionAttribute`:

```csharp
// Before
[Option(Description = "The template name")]
string? templateName = null

// After
[Option('t', Description = "The template name")]
string? templateName = null
```

When choosing short aliases:

- Use the first letter of the most distinctive word (`-t` for template, `-p` for path)
- Avoid conflicts with Cocona's built-in `-h` (help)
- Check existing commands for consistency (e.g., `-p` for `--project-path` across all commands)

## How Discovery Works

Convention tests use reflection to discover command methods:

1. Find all public classes in the `Keystone.Cli.Presentation` namespace ending with `Controller`
2. Get methods with `[Command]` attribute
3. Recursively resolve `[HasSubCommands]` targets and get their `*Async` methods

Discovery results are cached in a thread-safe `Lazy<MethodInfo[]>` field, so reflection runs
once regardless of how many convention tests execute. This approach ensures new commands are
automatically covered without updating the test.

## Adding New Convention Tests

When adding a new convention test:

1. Add a new test method to `CoconaCommandMethodConventionsTests.cs`
2. Use `LazyCommandMethods.Value` to access the cached command methods
3. Collect violations into a collection expression
4. Assert the collection is empty with a helpful message explaining the fix
5. Update this document with the new convention

Example structure:

```csharp
/// <summary>
/// Brief description of what this convention enforces.
/// </summary>
[Test]
public void ConventionName_ShouldDoSomething()
{
    string[] violations =
    [
        ..LazyCommandMethods.Value.SelectMany(method => /* find violations */)
    ];

    Assert.That(
        violations,
        Is.Empty,
        $"""
        Description of what went wrong.
        How to fix it.

        Violations:
        {string.Join(Environment.NewLine, violations.Select(v => $"- {v}"))}
        """
    );
}
```

The `LazyCommandMethods` field caches the discovered methods for reuse across tests, avoiding
redundant reflection overhead.
