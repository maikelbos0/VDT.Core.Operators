using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <inheritdoc/>
public class OperandStream<TValue> : IOperandStream<TValue> {
    private const int TRUE = 1;
    private const int FALSE = 0;

    private readonly ConcurrentDictionary<Subscription<TValue>, Func<TValue, CancellationToken, Task>> subscriptions = [];
    private readonly List<(TValue Value, CancellationToken CancellationToken)> publishedValues = [];
    private readonly object publishedValuesLock = new();
    private int startedValueGeneration = FALSE;

    //private readonly object generatedValuesTaskLock = new();
    //private Task<IEnumerable<TValue>>? generatedValuesTask;

    /// <inheritdoc/>
    public OperandStreamOptions<TValue> Options { get; init; }

    /// <inheritdoc/>
    public Task? ValueGenerationTask { get; private set; }

    /// <summary>
    /// Create an operand stream
    /// </summary>
    public OperandStream() : this(new()) { }

    /// <summary>
    /// Create an operand stream
    /// </summary>
    /// <param name="options">Options for this stream</param>
    public OperandStream(OperandStreamOptions<TValue> options) {
        Options = options;
    }

    /// <inheritdoc/>
    public Task Publish(TValue value)
        => Publish(value, CancellationToken.None);

    /// <inheritdoc/>
    public async Task Publish(TValue value, CancellationToken cancellationToken) {
        List<Task> publishTasks;

        if (Options.ReplayWhenSubscribing) {
            lock (publishedValuesLock) {
                publishedValues.Add((value, cancellationToken));

                publishTasks = subscriptions.Select(subscription => Publish(subscription.Key.PublishTask, subscription.Value, value, cancellationToken)).ToList();
            }
        }
        else {
            publishTasks = subscriptions.Select(subscription => Publish(subscription.Key.PublishTask, subscription.Value, value, cancellationToken)).ToList();
        }

        await Task.WhenAll(publishTasks);
    }

    private async Task Publish(Task previousPublishTask, Func<TValue, CancellationToken, Task> subscriber, TValue value, CancellationToken cancellationToken) {
        try {
            await previousPublishTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException) {
            // TODO we want to start when the previous task is done but reconsider how to best accomplish this
        }

        await subscriber(value, cancellationToken);
    }

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
        Subscription<TValue> subscription;

        if (Options.ReplayWhenSubscribing) {
            lock (publishedValuesLock) {
                subscription = new Subscription<TValue>(this, PublishInitialValues(subscriber, publishedValues.ToList()));

                subscriptions.TryAdd(subscription, subscriber);
            }
        }
        else {
            subscription = new Subscription<TValue>(this, PublishInitialValues(subscriber, []));

            subscriptions.TryAdd(subscription, subscriber);
        }

        if (Options.ValueGenerator != null && !Options.ReplayValueGeneratorWhenSubscribing && Interlocked.CompareExchange(ref startedValueGeneration, TRUE, FALSE) == FALSE) {
            ValueGenerationTask = GenerateValues(Options.ValueGenerator);
        }

        return subscription;
    }

    private async Task PublishInitialValues(Func<TValue, CancellationToken, Task> subscriber, IList<(TValue Value, CancellationToken CancellationToken)> publishedValues) {
        if (Options.ValueGenerator != null && Options.ReplayValueGeneratorWhenSubscribing) {
            await foreach (var value in Options.ValueGenerator()) {
                await subscriber(value, CancellationToken.None);
            }
        }

        foreach (var publishedValue in publishedValues) {
            await subscriber(publishedValue.Value, publishedValue.CancellationToken);
        }
    }

    private async Task GenerateValues(Func<IAsyncEnumerable<TValue>> valueGenerator) {
        await foreach (var value in valueGenerator()) {
            await Publish(value, CancellationToken.None);
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

    /// <inheritdoc/>
    public void Unsubscribe(Subscription<TValue> subscription) {
        if (subscription.OperandStream == this) {
            subscription.OperandStream = null;
            subscriptions.TryRemove(subscription, out _);
        }
    }

    /// <inheritdoc/>
    public void UnsubscribeAll() {
        foreach (var subscription in subscriptions.Keys) {
            subscription.OperandStream = null;
            subscriptions.TryRemove(subscription, out _);
        }
    }
}

/// <inheritdoc/>
public class OperandStream : OperandStream<Void> { }
