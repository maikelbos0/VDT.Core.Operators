using System;

namespace VDT.Core.Operators;

public static class OperandStreamExtensions {
    public static IOperandStream<TValue> Filter<TValue>(this IOperandStream<TValue> operandStream, Func<TValue, bool> predicate)
        => operandStream.Pipe(new Filter<TValue>(predicate));

    public static IOperandStream<TTransformedValue> Map<TValue, TTransformedValue>(this IOperandStream<TValue> operandStream, Func<TValue, TTransformedValue> func)
        => operandStream.Pipe(new Map<TValue, TTransformedValue>(func));

    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Debounce<TValue>(delayInMilliseconds));

    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Throttle<TValue>(delayInMilliseconds));

    public static IOperandStream<TValue> Flatten<TValue>(this IOperandStream<IOperandStream<TValue>> operandStream)
        => operandStream.Pipe(new Flatten<TValue>());

    // TODO add overloads
    // TODO maybe split into separate classes?
}
