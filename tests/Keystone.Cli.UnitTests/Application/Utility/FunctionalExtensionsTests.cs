using Keystone.Cli.Application.Utility;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Utility;

[TestFixture, Parallelizable(ParallelScope.All)]
public class FunctionalExtensionsTests
{
    [Test]
    public void With_ActionIsNull_ThrowsArgumentNullException()
    {
        var instance = new object();
        Action<object> action = null!;

        Assert.That(
            () => instance.With(action),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("action")
        );
    }

    [Test]
    public void With_ValidAction_ExecutesActionAndReturnsInstance()
    {
        var instance = new object();
        var action = Substitute.For<Action<object>>();

        var actual = instance.With(action);

        action.Received(1).Invoke(instance);
        Assert.That(actual, Is.SameAs(instance));
    }
}
