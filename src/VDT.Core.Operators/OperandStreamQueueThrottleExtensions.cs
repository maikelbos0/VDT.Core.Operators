using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public static class OperandStreamQueueThrottleExtensions {
    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayInMilliseconds));

    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, Func<int> delayFunc)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayFunc));

    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, Func<Task<int>> delayFunc)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayFunc));

    public static IOperandStream<TValue> QueueThrottle<TValue>(this IOperandStream<TValue> operandStream, Func<CancellationToken, Task<int>> delayFunc)
        => operandStream.Pipe(new QueueThrottle<TValue>(delayFunc));
}
