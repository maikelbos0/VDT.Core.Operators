using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Flatten<TValue> : IOperator<IOperandStream<TValue>, TValue> {
    public Task Execute(IOperandStream<TValue> value, IOperandStream<TValue> targetStream, CancellationToken _) {
        value.Subscribe((Func<TValue, CancellationToken, Task>)targetStream.Publish);

        return Task.CompletedTask;
    }
}
