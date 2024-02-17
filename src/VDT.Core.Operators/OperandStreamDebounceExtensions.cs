using System.Threading.Tasks;
using System.Threading;
using System;

namespace VDT.Core.Operators;

public static class OperandStreamDebounceExtensions {
    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Debounce<TValue>(delayInMilliseconds));

    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, Func<int> delayFunc)
        => operandStream.Pipe(new Debounce<TValue>(delayFunc));

    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, Func<Task<int>> delayFunc)
        => operandStream.Pipe(new Debounce<TValue>(delayFunc));

    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, Func<CancellationToken, Task<int>> delayFunc)
        => operandStream.Pipe(new Debounce<TValue>(delayFunc));
}
