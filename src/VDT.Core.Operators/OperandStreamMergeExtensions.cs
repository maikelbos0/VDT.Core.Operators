using System.Collections.Generic;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamMergeExtensions {
    /// <summary>
    /// Publish values received from two or more instances of <see cref="IOperandStream{TValue}"/> of the same <typeparamref name="TValue"/>
    /// </summary>
    /// <typeparam name="TValue">Type of values</typeparam>
    /// <param name="operandStream">First <see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="additionalStreams">Additional instances of <see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that all received values will be published to</returns>
    public static IOperandStream<TValue> Merge<TValue>(this IOperandStream<TValue> operandStream, params IOperandStream<TValue>[] additionalStreams)
        => operandStream.Pipe(new Merge<TValue>(), additionalStreams);
    
    /// <summary>
    /// Publish values received from two or more instances of <see cref="IOperandStream{TValue}"/> of the same <typeparamref name="TValue"/>
    /// </summary>
    /// <typeparam name="TValue">Type of values</typeparam>
    /// <param name="operandStream">First <see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="additionalStreams">Additional instances of <see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that all received values will be published to</returns>
    public static IOperandStream<TValue> Merge<TValue>(this IOperandStream<TValue> operandStream, IEnumerable<IOperandStream<TValue>> additionalStreams)
        => operandStream.Pipe(new Merge<TValue>(), additionalStreams);
}
