using Keystone.Cli.Domain;


namespace Keystone.Cli.UnitTests.Domain;

[TestFixture, Parallelizable(ParallelScope.All)]
public class TemplateTargetModelTests
{
    [Test]
    public void RepositoryName_ExtractsLastSegmentFromUrl()
    {
        var sut = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri("https://github.com/knight-owl-dev/template-core")
        );

        Assert.That(sut.RepositoryName, Is.EqualTo("template-core"));
    }

    [Test]
    public void RepositoryName_WhenUrlHasTrailingSlash_TrimsSlash()
    {
        var sut = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri("https://github.com/knight-owl-dev/template-core/")
        );

        Assert.That(sut.RepositoryName, Is.EqualTo("template-core"));
    }
}
