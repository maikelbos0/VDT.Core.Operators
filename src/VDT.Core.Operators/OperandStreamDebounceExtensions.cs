using System.Threading.Tasks;
using System.Threading;
using System;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamDebounceExtensions {
    /// <summary>
    /// Publish only the last value received from the <paramref name="operandStream"/> in succession after <paramref name="delayInMilliseconds"/>, discarding those values received while the delay had not finished
    /// </summary>
    /// <typeparam name="TValue">Type of value to debounce</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayInMilliseconds">Time in milliseconds to wait before publishing a value</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that debounced values will be published to</returns>
    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Debounce<TValue>(delayInMilliseconds));

    /// <summary>
    /// Publish only the last value received from the <paramref name="operandStream"/> in succession after the amount of milliseconds provided by <paramref name="delayFunc"/>, discarding those values received while the delay had not finished
    /// </summary>
    /// <typeparam name="TValue">Type of value to debounce</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait before publishing a value</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that debounced values will be published to</returns>
    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, Func<int> delayFunc)
        => operandStream.Pipe(new Debounce<TValue>(delayFunc));

    /// <summary>
    /// Publish only the last value received from the <paramref name="operandStream"/> in succession after the amount of milliseconds provided by <paramref name="delayFunc"/>, discarding those values received while the delay had not finished
    /// </summary>
    /// <typeparam name="TValue">Type of value to debounce</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait before publishing a value</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that debounced values will be published to</returns>
    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, Func<Task<int>> delayFunc)
        => operandStream.Pipe(new Debounce<TValue>(delayFunc));

    /// <summary>
    /// Publish only the last value received from the <paramref name="operandStream"/> in succession after the amount of milliseconds provided by <paramref name="delayFunc"/>, discarding those values received while the delay had not finished
    /// </summary>
    /// <typeparam name="TValue">Type of value to debounce</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait before publishing a value</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that debounced values will be published to</returns>
    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, Func<CancellationToken, Task<int>> delayFunc)
        => operandStream.Pipe(new Debounce<TValue>(delayFunc));
}
