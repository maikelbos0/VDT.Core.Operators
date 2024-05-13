using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <inheritdoc/>
public class OperandStream<TValue> : IOperandStream<TValue> {
    private readonly ConcurrentDictionary<object, List<Func<TValue, CancellationToken, Task>>> subscribers = [];

    /// <inheritdoc/>
    public Task Publish(TValue value)
        => Publish(value, CancellationToken.None);

    /// <inheritdoc/>
    public Task Publish(TValue value, CancellationToken cancellationToken)
        => Task.WhenAll(subscribers.Values.SelectMany(subscribers => subscribers.Select(subscriber => subscriber(value, cancellationToken))));

    /// <inheritdoc/>
    public void Subscribe(Action subscriber)
        => Subscribe(subscriber, (_, _) => {
            subscriber();
            return Task.CompletedTask;
        });

    /// <inheritdoc/>
    public void Subscribe(Action<TValue> subscriber)
        => Subscribe(subscriber, (value, _) => {
            subscriber(value);
            return Task.CompletedTask;
        });

    /// <inheritdoc/>
    public void Subscribe(Func<Task> subscriber)
        => Subscribe(subscriber, (_, _) => subscriber());

    /// <inheritdoc/>
    public void Subscribe(Func<TValue, Task> subscriber)
        => Subscribe(subscriber, (value, _) => subscriber(value));

    /// <inheritdoc/>
    public void Subscribe(Func<CancellationToken, Task> subscriber)
        => Subscribe(subscriber, (_, cancellationToken) => subscriber(cancellationToken));

    /// <inheritdoc/>
    public void Subscribe(Func<TValue, CancellationToken, Task> subscriber)
        => Subscribe(subscriber, subscriber);

    private void Subscribe(object key, Func<TValue, CancellationToken, Task> subscriber) {
        subscribers.AddOrUpdate(
            key,
            key => new List<Func<TValue, CancellationToken, Task>>() { subscriber },
            (key, subscribers) => {
                subscribers.Add(subscriber);
                return subscribers;
            }
        );
    }

    /// <inheritdoc/>
    public IOperandStream<TTransformedValue> Pipe<TTransformedValue>(IOperator<TValue, TTransformedValue> op) {
        var targetStream = new OperandStream<TTransformedValue>();

        Subscribe(async (value, cancellationToken) => await op.Execute(value, targetStream, cancellationToken));

        return targetStream;
    }

    /// <inheritdoc/>
    public IOperandStream<TTransformedValue> Pipe<TTransformedValue, TInitializationData>(IOperator<TValue, TTransformedValue, TInitializationData> op, TInitializationData initializationData) {
        var targetStream = new OperandStream<TTransformedValue>();

        op.Initialize(targetStream, initializationData);

        Subscribe(async (value, cancellationToken) => await op.Execute(value, targetStream, cancellationToken));

        return targetStream;
    }
}

/// <inheritdoc/>
public class OperandStream : OperandStream<Void> { }
