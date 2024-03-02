using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandstreamFilterExtensions {
    /// <summary>
    /// Publish only those received values that match the <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="TValue">Type of value to filter</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="predicate">Predicate against which values will be tested</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that values that match the <paramref name="predicate"/> will be published to</returns>
    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, bool> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));

    /// <summary>
    /// Publish only those received values that match the <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="TValue">Type of value to filter</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="predicate">Predicate against which values will be tested</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that values that match the <paramref name="predicate"/> will be published to</returns>
    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, Task<bool>> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));

    /// <summary>
    /// Publish only those received values that match the <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="TValue">Type of value to filter</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="predicate">Predicate against which values will be tested</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that values that match the <paramref name="predicate"/> will be published to</returns>
    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task<bool>> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));
}
