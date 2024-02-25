using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamMapExtensions {
    /// <summary>
    /// Publish values transformed from <typeparamref name="TValue"/> into <typeparamref name="TTransformedValue"/> by the supplied transformation method
    /// </summary>
    /// <typeparam name="TValue">Type of received values</typeparam>
    /// <typeparam name="TTransformedValue">Type of values to publish</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="func">Method that transforms received values into values to publish</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that transformed values will be published to</returns>
    public static IOperandStream<TTransformedValue> Map<TValue, TTransformedValue>(this IOperandStream<TValue> operandStream, Func<TValue, TTransformedValue> func)
        => operandStream.Pipe(new Map<TValue, TTransformedValue>(func));

    /// <summary>
    /// Publish values transformed from <typeparamref name="TValue"/> into <typeparamref name="TTransformedValue"/> by the supplied transformation method
    /// </summary>
    /// <typeparam name="TValue">Type of received values</typeparam>
    /// <typeparam name="TTransformedValue">Type of values to publish</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="func">Method that transforms received values into values to publish</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that transformed values will be published to</returns>
    public static IOperandStream<TTransformedValue> Map<TValue, TTransformedValue>(this IOperandStream<TValue> operandStream, Func<TValue, Task<TTransformedValue>> func)
        => operandStream.Pipe(new Map<TValue, TTransformedValue>(func));

    /// <summary>
    /// Publish values transformed from <typeparamref name="TValue"/> into <typeparamref name="TTransformedValue"/> by the supplied transformation method
    /// </summary>
    /// <typeparam name="TValue">Type of received values</typeparam>
    /// <typeparam name="TTransformedValue">Type of values to publish</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="func">Method that transforms received values into values to publish</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that transformed values will be published to</returns>
    public static IOperandStream<TTransformedValue> Map<TValue, TTransformedValue>(this IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task<TTransformedValue>> func)
        => operandStream.Pipe(new Map<TValue, TTransformedValue>(func));
}
