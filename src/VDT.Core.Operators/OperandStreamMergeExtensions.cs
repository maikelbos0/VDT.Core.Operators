using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace VDT.Core.Operators;

public static class OperandStreamMergeExtensions {
    public static IOperandStream<TValue> Merge<TValue>(this IOperandStream<TValue> operandStream, params IOperandStream<TValue>[] otherStreams)
        => Merge(operandStream, otherStreams.AsEnumerable());

    public static IOperandStream<TValue> Merge<TValue>(this IOperandStream<TValue> operandStream, IEnumerable<IOperandStream<TValue>> otherStreams) {
        var targetStream = new OperandStream<TValue>();
        var subscriber = (Func<TValue, CancellationToken, Task>)targetStream.Publish;

        operandStream.Subscribe(subscriber);
        foreach (var otherStream in otherStreams) {
            otherStream.Subscribe(subscriber);
        }

        return targetStream;
    }
}
