using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <inheritdoc/>
public class OperandStream<TValue> : IOperandStream<TValue> {
    private class Subscription {
        public Func<TValue, CancellationToken, Task> Subscriber { get; }
        public int Count { get; set; } = 1;
        public Subscription(Func<TValue, CancellationToken, Task> subscriber) {
            Subscriber = subscriber;
        }
    }

    private readonly object subscriptionsLock = new();
    private readonly Dictionary<object, Subscription> subscriptions = [];

    /// <inheritdoc/>
    public Task Publish(TValue value)
        => Publish(value, CancellationToken.None);

    /// <inheritdoc/>
    public Task Publish(TValue value, CancellationToken cancellationToken)
        => Task.WhenAll(subscriptions.Values.SelectMany(subscription => Enumerable.Repeat(subscription, subscription.Count).Select(subscription => subscription.Subscriber(value, cancellationToken))));

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
        lock (subscriptionsLock) {
            if (subscriptions.TryGetValue(key, out var subscription)) {
                subscription.Count++;
            }
            else {
                subscriptions.Add(key, new Subscription(subscriber));
            }
        }
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
