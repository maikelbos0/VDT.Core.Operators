using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <inheritdoc/>
public class OperandStream<TValue> : IOperandStream<TValue> {
    private readonly List<Func<TValue, CancellationToken, Task>> subscribers = [];

    /// <inheritdoc/>
    public Task Write(TValue value)
        => Write(value, CancellationToken.None);

    /// <inheritdoc/>
    public Task Write(TValue value, CancellationToken cancellationToken)
        => Task.WhenAll(subscribers.Select(subscriber => subscriber(value, cancellationToken)));

    /// <inheritdoc/>
    public void Subscribe(Action subscriber)
        => Subscribe((_, _) => {
            subscriber();
            return Task.CompletedTask;
        });

    /// <inheritdoc/>
    public void Subscribe(Action<TValue> subscriber)
        => subscribers.Add((value, _) => {
            subscriber(value);
            return Task.CompletedTask;
        });

    /// <inheritdoc/>
    public void Subscribe(Func<Task> subscriber)
        => Subscribe((_, _) => subscriber());

    /// <inheritdoc/>
    public void Subscribe(Func<TValue, Task> subscriber)
        => subscribers.Add((value, _) => subscriber(value));

    /// <inheritdoc/>
    public void Subscribe(Func<CancellationToken, Task> subscriber)
        => Subscribe((_, cancellationToken) => subscriber(cancellationToken));

    /// <inheritdoc/>
    public void Subscribe(Func<TValue, CancellationToken, Task> subscriber)
        => subscribers.Add(subscriber);

    /// <inheritdoc/>
    public IOperandStream<TTransformedValue> Pipe<TTransformedValue>(IOperator<TValue, TTransformedValue> op) {
        var targetStream = new OperandStream<TTransformedValue>();

        Subscribe(async (value, cancellationToken) => await op.Execute(value, targetStream, cancellationToken));

        return targetStream;
    }
}
