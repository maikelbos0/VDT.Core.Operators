using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamThrottleExtensions {
    /// <summary>
    /// Pause at least the specified delay in between publishing values, discarding older values when a new value is received while awaiting the delay
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayInMilliseconds">Time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Throttle<TValue>(delayInMilliseconds));

    /// <summary>
    /// Pause at least the specified delay in between publishing values, discarding older values when a new value is received while awaiting the delay
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, Func<int> delayFunc)
        => operandStream.Pipe(new Throttle<TValue>(delayFunc));

    /// <summary>
    /// Pause at least the specified delay in between publishing values, discarding older values when a new value is received while awaiting the delay
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, Func<Task<int>> delayFunc)
        => operandStream.Pipe(new Throttle<TValue>(delayFunc));

    /// <summary>
    /// Pause at least the specified delay in between publishing values, discarding older values when a new value is received while awaiting the delay
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, Func<CancellationToken, Task<int>> delayFunc)
        => operandStream.Pipe(new Throttle<TValue>(delayFunc));
}
