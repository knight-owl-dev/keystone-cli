using System.Text.RegularExpressions;
using Keystone.Cli.Domain;
using Keystone.Cli.UnitTests.TestUtilities;


namespace Keystone.Cli.UnitTests.Docs;

[TestFixture, Parallelizable(ParallelScope.All)]
public partial class ManPageDateTagTests
{
    private static readonly string[] ValidEnglishMonths =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    ];

    [GeneratedRegex(@"^\.Dd ([A-Za-z]+) (\d{4})$")]
    private static partial Regex DdTagPattern();

    [Test]
    public void ManPage_HasExactlyOneDdTag_WithValidEnglishMonthAndYear()
    {
        var manPagePath = GetManPagePath();
        var lines = File.ReadAllLines(manPagePath);

        var ddLines = lines
            .Select((line, index) => (Line: line, LineNumber: index + 1))
            .Where(x => x.Line.StartsWith(".Dd"))
            .ToList();

        Assert.That(ddLines, Has.Count.EqualTo(1), "Expected exactly one .Dd line in the man page");

        var (ddLine, lineNumber) = ddLines[0];
        var match = DdTagPattern().Match(ddLine);

        Assert.That(
            match.Success,
            Is.True,
            $"Line {lineNumber}: .Dd tag does not match expected pattern '.Dd Month YYYY'. Actual: '{ddLine}'"
        );

        var month = match.Groups[1].Value;
        Assert.That(
            ValidEnglishMonths,
            Does.Contain(month),
            $"Line {lineNumber}: Month '{month}' is not a valid English month name"
        );

        var year = int.Parse(match.Groups[2].Value);
        Assert.That(
            year,
            Is.GreaterThanOrEqualTo(CliInfo.InceptionYear).And.LessThanOrEqualTo(CliInfo.CurrentYear),
            $"Line {lineNumber}: Year {year} is outside reasonable range ({CliInfo.InceptionYear}-{CliInfo.CurrentYear})"
        );
    }

    private static string GetManPagePath()
    {
        var manPagePath = Path.Combine(RepoPathResolver.GetRepoRoot(), "docs", "man", "man1", "keystone-cli.1");

        return ! File.Exists(manPagePath)
            ? throw new FileNotFoundException($"Man page not found at expected path: {manPagePath}")
            : manPagePath;
    }
}
