using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class OperandStream<TValue> : IOperandStream<TValue> {
    private readonly List<Func<TValue, CancellationToken, Task>> subscribers = [];

    public Task Write(TValue value)
        => Write(value, CancellationToken.None);

    public Task Write(TValue value, CancellationToken cancellationToken)
        => Task.WhenAll(subscribers.Select(subscriber => subscriber(value, cancellationToken)));

    public void Subscribe(Action<TValue> subscriber)
        => subscribers.Add((value, _) => {
            subscriber(value);
            return Task.CompletedTask;
        });

    public void Subscribe(Func<TValue, Task> subscriber)
        => subscribers.Add((value, _) => subscriber(value));

    public void Subscribe(Func<TValue, CancellationToken, Task> subscriber)
        => subscribers.Add(subscriber);

    public IOperandStream<TTransformedValue> Pipe<TTransformedValue>(IOperator<TValue, TTransformedValue> op) {
        var targetStream = new OperandStream<TTransformedValue>();

        Subscribe(async (value, cancellationToken) => await op.Execute(value, targetStream, cancellationToken));

        return targetStream;
    }
}
