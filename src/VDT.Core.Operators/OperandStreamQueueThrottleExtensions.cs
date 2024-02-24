using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamQueueThrottleExtensions {
    /// <summary>
    /// Pause at least the specified delay in between publishing values, queueing received values
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayInMilliseconds">Time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayInMilliseconds));

    /// <summary>
    /// Pause at least the specified delay in between publishing values, queueing received values
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, Func<int> delayFunc)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayFunc));

    /// <summary>
    /// Pause at least the specified delay in between publishing values, queueing received values
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, Func<Task<int>> delayFunc)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayFunc));

    /// <summary>
    /// Pause at least the specified delay in between publishing values, queueing received values
    /// </summary>
    /// <typeparam name="TValue">Type of value to throttle</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that throttled values will be published to</returns>
    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, Func<CancellationToken, Task<int>> delayFunc)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayFunc));
}
