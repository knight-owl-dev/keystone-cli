using Keystone.Cli.Application.Utility;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Utility;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EnumerableAsyncExtensionsTests
{
    [Test]
    public async Task AggregateAsync_WhenSourceIsEmpty_ReturnsSeedAsync()
    {
        const int seed = 12345;

        int[] source = [];
        var func = Substitute.For<Func<int, int, CancellationToken, Task<int>>>();

        var actual = await source.AggregateAsync(seed, func);

        Assert.That(actual, Is.EqualTo(seed));
    }

    [Test]
    public async Task AggregateAsync_WhenSourceHasElements_AppliesAccumulatorAsync()
    {
        int[] source = [1, 2, 3];

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var func = Substitute.For<Func<int, int, CancellationToken, Task<int>>>();
        func.Invoke(Arg.Any<int>(), Arg.Any<int>(), cancellationToken)
            .Returns(ci => Task.FromResult((int) ci[0] + (int) ci[1]));

        var actual = await source.AggregateAsync(0, func, cancellationToken);

        Assert.That(actual, Is.EqualTo(6)); // 0+1=1, 1+2=3, 3+3=6
        await func.Received(3).Invoke(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AggregateAsync_WithResultSelector_AppliesSelectorToFinalAccumulatorAsync()
    {
        int[] source = [2, 3];

        var func = Substitute.For<Func<int, int, CancellationToken, Task<int>>>();
        func.Invoke(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult((int) ci[0] * (int) ci[1]));

        var selector = Substitute.For<Func<int, string>>();
        selector.Invoke(6).Returns("six");

        var actual = await source.AggregateAsync(1, func, selector);

        Assert.That(actual, Is.EqualTo("six")); // 1*2=2, 2*3=6, selector(6)="six"

        await func.Received(2).Invoke(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        selector.Received(1).Invoke(6);
    }

    [Test]
    public void AggregateAsync_ThrowsIfSourceIsNull()
    {
        IEnumerable<int> source = null!;
        var func = Substitute.For<Func<int, int, CancellationToken, Task<int>>>();

        Assert.That(
            () => source.AggregateAsync(seed: 0, func),
            Throws.ArgumentNullException
        );
    }

    [Test]
    public void AggregateAsync_ThrowsIfFuncIsNull()
    {
        int[] source = [1];
        Func<int, int, CancellationToken, Task<int>> func = null!;

        Assert.That(
            () => source.AggregateAsync(seed: 0, func),
            Throws.ArgumentNullException
        );
    }

    [Test]
    public void AggregateAsync_WithResultSelector_ThrowsIfSelectorIsNull()
    {
        int[] source = [1];
        var func = Substitute.For<Func<int, int, CancellationToken, Task<int>>>();
        Func<int, string> selector = null!;

        Assert.That(
            () => source.AggregateAsync(seed: 0, func, selector),
            Throws.ArgumentNullException
        );
    }

    [Test]
    public void AggregateAsync_ThrowsIfCancelled()
    {
        int[] source = [1];
        var func = Substitute.For<Func<int, int, CancellationToken, Task<int>>>();

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        cts.Cancel();

        Assert.That(
            () => source.AggregateAsync(seed: 0, func, cancellationToken),
            Throws.TypeOf<OperationCanceledException>()
        );
    }

    [Test]
    public void AggregateAsync_WithResultSelector_ThrowsIfCancelled()
    {
        int[] source = [1];
        var func = Substitute.For<Func<int, int, CancellationToken, Task<int>>>();
        Func<int, string> selector = x => x.ToString();

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        cts.Cancel();

        Assert.That(
            () => source.AggregateAsync(seed: 0, func, selector, cancellationToken),
            Throws.TypeOf<OperationCanceledException>()
        );
    }
}
