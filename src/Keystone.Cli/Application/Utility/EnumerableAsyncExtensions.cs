namespace Keystone.Cli.Application.Utility;

/// <summary>
/// Async extension methods for working with <see cref="IEnumerable{T}"/> instances.
/// </summary>
public static class EnumerableAsyncExtensions
{
    /// <summary>
    /// Aggregates the elements of a sequence asynchronously by applying
    /// an asynchronous accumulator function over each element and then
    /// applying a synchronous result selector to the final accumulator value.
    /// </summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="seed">The initial value for the accumulator.</param>
    /// <param name="func">The aggregate function.</param>
    /// <param name="resultSelector">The result selector function.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TSource">The type of items in the source sequence.</typeparam>
    /// <typeparam name="TAccumulate">The type of accumulator value.</typeparam>
    /// <typeparam name="TResult">The type of result value.</typeparam>
    /// <returns>
    /// The final result value after processing all elements in the source sequence;
    /// otherwise, the result of applying <paramref name="resultSelector"/> to the seed value
    /// if the source sequence is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/>, <paramref name="func"/>, or <paramref name="resultSelector"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided <paramref name="cancellationToken"/>.
    /// </exception>
    public static async Task<TResult> AggregateAsync<TSource, TAccumulate, TResult>(
        this IEnumerable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, CancellationToken, Task<TAccumulate>> func,
        Func<TAccumulate, TResult> resultSelector,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(func);
        ArgumentNullException.ThrowIfNull(resultSelector);

        var accumulator = await source.AggregateAsync(seed, func, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        return resultSelector(accumulator);
    }

    /// <summary>
    /// Aggregates the elements of a sequence asynchronously by applying
    /// an asynchronous accumulator function over each element.
    /// </summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="seed">The initial value for the accumulator.</param>
    /// <param name="func">The aggregate function.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TSource">The type of items in the source sequence.</typeparam>
    /// <typeparam name="TAccumulator">The type of accumulator value.</typeparam>
    /// <returns>
    /// The final accumulator value after processing all elements in the source sequence;
    /// otherwise, the seed value if the source sequence is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="func"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided <paramref name="cancellationToken"/>.
    /// </exception>
    public static async Task<TAccumulator> AggregateAsync<TSource, TAccumulator>(
        this IEnumerable<TSource> source,
        TAccumulator seed,
        Func<TAccumulator, TSource, CancellationToken, Task<TAccumulator>> func,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(func);

        var accumulator = seed;
        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            accumulator = await func(accumulator, item, cancellationToken).ConfigureAwait(false);
        }

        return accumulator;
    }
}
