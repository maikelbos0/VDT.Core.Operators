using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public static class OperandstreamFilterExtensions {
    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, bool> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));

    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, Task<bool>> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));

    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task<bool>> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));
}
