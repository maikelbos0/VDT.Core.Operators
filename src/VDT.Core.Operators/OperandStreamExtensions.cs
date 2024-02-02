using System;

namespace VDT.Core.Operators;

public static class OperandStreamExtensions {
    public static OperandStream<TValue> Filter<TValue>(this OperandStream<TValue> operandStream, Func<TValue, bool> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));

    public static OperandStream<TNewValue> Map<TValue, TNewValue>(this OperandStream<TValue> operandStream, Func<TValue, TNewValue> mapFunc)
        => operandStream.Pipe(new Map<TValue, TNewValue>(mapFunc));
}
