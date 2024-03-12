using System.Collections.Generic;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamIterateExtensions {
    /// <summary>
    /// Iterate over received values of <see cref="IEnumerable{T}"/> of <typeparamref name="TValue"/> and publish each item
    /// </summary>
    /// <typeparam name="TValue">Type of values</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that values will be published to</returns>
    public static IOperandStream<TValue> Iterate<TValue>(this IOperandStream<IEnumerable<TValue>> operandStream)
        => operandStream.Pipe(new Iterate<TValue>());
}
