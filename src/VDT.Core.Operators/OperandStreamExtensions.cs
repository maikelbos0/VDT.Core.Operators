using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace VDT.Core.Operators;

public static class OperandStreamExtensions {
    public static Task Publish(this IOperandStream<Void> operandStream) 
        => operandStream.Publish(new Void());

    public static Task Publish(this IOperandStream<Void> operandStream, CancellationToken cancellationToken) 
        => operandStream.Publish(new Void(), cancellationToken);

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

    // TODO add overloads
    // TODO maybe split into separate classes?
}
