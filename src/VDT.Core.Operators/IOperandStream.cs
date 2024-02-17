using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Represents a stream of values that can be subscribed to and transformed
/// </summary>
/// <typeparam name="TValue">Type of the values</typeparam>
public interface IOperandStream<TValue> {
    /// <summary>
    /// Publish a value to this stream
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once the value has been consumed by all subscribers and operators</returns>
    Task Publish(TValue value);

    /// <summary>
    /// Publish a value to this stream
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once the value has been consumed by all subscribers and operators</returns>
    Task Publish(TValue value, CancellationToken cancellationToken);

    /// <summary>
    /// Subscribe a method to execute when a stream is published to
    /// </summary>
    /// <param name="subscriber">Method that gets executed</param>
    void Subscribe(Action subscriber);

    /// <summary>
    /// Subscribe a method to receive values published to this stream
    /// </summary>
    /// <param name="subscriber">Method that handles the received value</param>
    void Subscribe(Action<TValue> subscriber);

    /// <summary>
    /// Subscribe a method to execute when a stream is published to
    /// </summary>
    /// <param name="subscriber">Method that gets executed</param>
    void Subscribe(Func<Task> subscriber);

    /// <summary>
    /// Subscribe a method to receive values published to this stream
    /// </summary>
    /// <param name="subscriber">Method that handles the received value</param>
    void Subscribe(Func<TValue, Task> subscriber);

    /// <summary>
    /// Subscribe a method to execute when a stream is published to
    /// </summary>
    /// <param name="subscriber">Method that gets executed</param>
    void Subscribe(Func<CancellationToken, Task> subscriber);

    /// <summary>
    /// Subscribe a method to receive values published to this stream
    /// </summary>
    /// <param name="subscriber">Method that handles the received value</param>
    void Subscribe(Func<TValue, CancellationToken, Task> subscriber);

    /// <summary>
    /// Pass values published to this stream to the supplied operator for transformation and publish them to a target stream
    /// </summary>
    /// <typeparam name="TTransformedValue">Type of the value after transformation</typeparam>
    /// <param name="op">Operator that will publish values to the target stream</param>
    /// <returns>The target stream the supplied operator will publish transformed values to</returns>
    IOperandStream<TTransformedValue> Pipe<TTransformedValue>(IOperator<TValue, TTransformedValue> op);

    /// <summary>
    /// Pass values published to this stream to the supplied operator for transformation and publish them to a target stream
    /// </summary>
    /// <typeparam name="TTransformedValue">Type of the value after transformation</typeparam>
    /// <typeparam name="TInitializationData">Type of the initialization data the operator accepts for initialization</typeparam>
    /// <param name="op">Operator that will publish values to the target stream</param>
    /// <param name="initializationData">Data for the operator to initialize the target stream with</param>
    /// <returns>The target stream the supplied operator will publish transformed values to</returns>
    IOperandStream<TTransformedValue> Pipe<TTransformedValue, TInitializationData>(IOperator<TValue, TTransformedValue, TInitializationData> op, TInitializationData initializationData);
}
