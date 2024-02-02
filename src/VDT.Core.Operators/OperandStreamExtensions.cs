using System;

namespace VDT.Core.Operators;

public static class OperandStreamExtensions {
    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, bool> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));

    public static IOperandStream<TNewValue> Map<TValue, TNewValue>(this IOperandStream<TValue> operandStream, Func<TValue, TNewValue> mapFunc)
        => operandStream.Pipe(new Map<TValue, TNewValue>(mapFunc));

    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Debounce<TValue>(delayInMilliseconds));
}
