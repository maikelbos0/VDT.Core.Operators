using System;

namespace VDT.Core.Operators;

// TODO add overloads
public static class OperandStreamMapExtensions {
    public static IOperandStream<TTransformedValue> Map<TValue, TTransformedValue>(this IOperandStream<TValue> operandStream, Func<TValue, TTransformedValue> func)
        => operandStream.Pipe(new Map<TValue, TTransformedValue>(func));
}
