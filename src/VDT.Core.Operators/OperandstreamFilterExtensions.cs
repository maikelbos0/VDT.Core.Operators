using System;

namespace VDT.Core.Operators;

// TODO add overloads
public static class OperandstreamFilterExtensions {
    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, bool> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));
}
