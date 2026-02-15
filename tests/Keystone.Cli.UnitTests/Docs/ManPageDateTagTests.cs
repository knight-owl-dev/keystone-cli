using System.Globalization;
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

    [GeneratedRegex(@"^\.Dd ([A-Za-z]+) 1, (\d{4})$")]
    private static partial Regex DdTagPattern();

    [Test]
    public void ManPage_HasExactlyOneDdTag_WithValidEnglishMonthAndYear()
    {
        var localDate = CliInfo.GetLocalDate();

        var manPagePath = GetManPagePath();
        var lines = File.ReadAllLines(manPagePath);

        var ddLines = lines
            .Select((line, index) => (Line: line, LineNumber: index + 1))
            .Where(x => x.Line.StartsWith(".Dd", StringComparison.InvariantCulture))
            .ToList();

        Assert.That(ddLines, Has.Count.EqualTo(1), "Expected exactly one .Dd line in the man page");

        var (ddLine, lineNumber) = ddLines[0];
        var match = DdTagPattern().Match(ddLine);

        Assert.That(
            match.Success,
            Is.True,
            $"Line {lineNumber}: .Dd tag does not match expected pattern '.Dd Month 1, YYYY'. Actual: '{ddLine}'"
        );

        var monthName = match.Groups[1].Value;
        var monthIndex = Array.IndexOf(ValidEnglishMonths, monthName);
        Assert.That(
            monthIndex,
            Is.GreaterThanOrEqualTo(0),
            $"Line {lineNumber}: Month '{monthName}' is not a valid English month name"
        );

        var month = monthIndex + 1;
        var year = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
        Assert.That(
            year,
            Is.GreaterThanOrEqualTo(CliInfo.InceptionYear).And.LessThanOrEqualTo(localDate.Year),
            $"Line {lineNumber}: Year {year} is outside reasonable range ({CliInfo.InceptionYear}-{localDate.Year})"
        );

        if (year == localDate.Year)
        {
            Assert.That(
                month,
                Is.LessThanOrEqualTo(localDate.Month),
                $"Line {lineNumber}: Month '{monthName}' ({month}) is in the future for year {year} (current month: {localDate.Month})"
            );
        }
    }

    private static string GetManPagePath()
    {
        var manPagePath = Path.Combine(RepoPathResolver.GetRepoRoot(), "docs", "man", "man1", "keystone-cli.1");

        return !File.Exists(manPagePath)
            ? throw new FileNotFoundException($"Man page not found at expected path: {manPagePath}")
            : manPagePath;
    }
}
