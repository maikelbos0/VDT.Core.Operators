using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <inheritdoc/>
public class OperandStream<TValue> : IOperandStream<TValue> {
    private readonly object subscriptionsLock = new();
    private readonly Dictionary<Subscription<TValue>, Func<TValue, CancellationToken, Task>> subscriptions = [];

    /// <inheritdoc/>
    public Task Publish(TValue value)
        => Publish(value, CancellationToken.None);

    /// <inheritdoc/>
    public Task Publish(TValue value, CancellationToken cancellationToken)
        => Task.WhenAll(subscriptions.Values.Select(subscriber => subscriber(value, cancellationToken)));

    /// <inheritdoc/>
    public Subscription<TValue> Subscribe(Action subscriber)
        => Subscribe((_, _) => {
            subscriber();
            return Task.CompletedTask;
        });

    /// <inheritdoc/>
    public Subscription<TValue> Subscribe(Action<TValue> subscriber)
        => Subscribe((value, _) => {
            subscriber(value);
            return Task.CompletedTask;
        });

    /// <inheritdoc/>
    public Subscription<TValue> Subscribe(Func<Task> subscriber)
        => Subscribe((_, _) => subscriber());

    /// <inheritdoc/>
    public Subscription<TValue> Subscribe(Func<TValue, Task> subscriber)
        => Subscribe((value, _) => subscriber(value));

    /// <inheritdoc/>
    public Subscription<TValue> Subscribe(Func<CancellationToken, Task> subscriber)
        => Subscribe((_, cancellationToken) => subscriber(cancellationToken));

    /// <inheritdoc/>
    public Subscription<TValue> Subscribe(Func<TValue, CancellationToken, Task> subscriber) {
        lock (subscriptionsLock) {
            var subscription = new Subscription<TValue>(this);

            subscriptions.Add(subscription, subscriber);

            return subscription;
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
