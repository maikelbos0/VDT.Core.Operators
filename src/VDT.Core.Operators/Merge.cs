using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Merge<TValue> : IOperator<TValue, TValue, IEnumerable<IOperandStream<TValue>>> {
    public void Initialize(IOperandStream<TValue> targetStream, IEnumerable<IOperandStream<TValue>> otherStreams) {
        var subscriber = (Func<TValue, CancellationToken, Task>)targetStream.Publish;

        foreach (var otherStream in otherStreams) {
            otherStream.Subscribe(subscriber);
        }
    }

    public Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        return targetStream.Publish(value, cancellationToken);
    }
}
