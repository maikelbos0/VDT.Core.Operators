using System.Collections.Generic;

namespace VDT.Core.Operators;

public static class OperandStreamMergeExtensions {
    public static IOperandStream<TValue> Merge<TValue>(this IOperandStream<TValue> operandStream, params IOperandStream<TValue>[] additionalStreams)
        => operandStream.Pipe(new Merge<TValue>(), additionalStreams);

    public static IOperandStream<TValue> Merge<TValue>(this IOperandStream<TValue> operandStream, IEnumerable<IOperandStream<TValue>> additionalStreams)
        => operandStream.Pipe(new Merge<TValue>(), additionalStreams);
}
