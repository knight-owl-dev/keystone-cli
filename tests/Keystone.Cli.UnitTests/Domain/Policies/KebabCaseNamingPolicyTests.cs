using Keystone.Cli.Domain.Policies;


namespace Keystone.Cli.UnitTests.Domain.Policies;

[TestFixture, Parallelizable(ParallelScope.All)]
public class KebabCaseNamingPolicyTests
{
    [TestCase("My Project_Name+Test", ExpectedResult = "my-project-name-test")]
    [TestCase(" Already--Kebab --Case ", ExpectedResult = "already-kebab-case")]
    [TestCase("", ExpectedResult = "")]
    [TestCase("   ", ExpectedResult = "")]
    [TestCase("simple", ExpectedResult = "simple")]
    [TestCase("Complex  Name_With.Mix-ed+Delimiters", ExpectedResult = "complex-name-with-mix-ed-delimiters")]
    public string ToKebabCase_ReturnsNormalizedInput(string input)
        => KebabCaseNamingPolicy.ToKebabCase(input);
}
