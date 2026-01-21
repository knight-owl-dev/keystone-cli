using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.UnitTests.Domain.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectNotLoadedExceptionTests
{
    [Test]
    public void Constructor_WithMessage_SetsMessage()
    {
        const string message = "Custom error message";

        var exception = new ProjectNotLoadedException(message);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Null);
        }
    }

    [Test]
    public void Constructor_Parameterless_SetsDefaultMessage()
    {
        var exception = new ProjectNotLoadedException();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception.Message, Is.EqualTo("The project is not loaded."));
            Assert.That(exception.InnerException, Is.Null);
        }
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
        const string message = "Outer error message";
        var innerException = new InvalidOperationException("Inner error");

        var exception = new ProjectNotLoadedException(message, innerException);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }
    }
}
