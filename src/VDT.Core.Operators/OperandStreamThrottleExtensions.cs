using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public static class OperandStreamThrottleExtensions {
    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Throttle<TValue>(delayInMilliseconds));

    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, Func<int> delayFunc)
        => operandStream.Pipe(new Throttle<TValue>(delayFunc));

    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, Func<Task<int>> delayFunc)
        => operandStream.Pipe(new Throttle<TValue>(delayFunc));

    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, Func<CancellationToken, Task<int>> delayFunc)
        => operandStream.Pipe(new Throttle<TValue>(delayFunc));
}
