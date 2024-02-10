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
    /// Write a value to this stream
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once the value has been consumed by all subscribers and operators</returns>
    Task Write(TValue value);

    /// <summary>
    /// Write a value to this stream
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once the value has been consumed by all subscribers and operators</returns>
    Task Write(TValue value, CancellationToken cancellationToken);

    /// <summary>
    /// Subscribe a method to execute when a stream is written to
    /// </summary>
    /// <param name="subscriber">Method that gets executed</param>
    void Subscribe(Action subscriber);

    /// <summary>
    /// Subscribe a method to receive values written to this stream
    /// </summary>
    /// <param name="subscriber">Method that handles the received value</param>
    void Subscribe(Action<TValue> subscriber);

    /// <summary>
    /// Subscribe a method to execute when a stream is written to
    /// </summary>
    /// <param name="subscriber">Method that gets executed</param>
    void Subscribe(Func<Task> subscriber);

    /// <summary>
    /// Subscribe a method to receive values written to this stream
    /// </summary>
    /// <param name="subscriber">Method that handles the received value</param>
    void Subscribe(Func<TValue, Task> subscriber);

    /// <summary>
    /// Subscribe a method to execute when a stream is written to
    /// </summary>
    /// <param name="subscriber">Method that gets executed</param>
    void Subscribe(Func<CancellationToken, Task> subscriber);

    /// <summary>
    /// Subscribe a method to receive values written to this stream
    /// </summary>
    /// <param name="subscriber">Method that handles the received value</param>
    void Subscribe(Func<TValue, CancellationToken, Task> subscriber);

    /// <summary>
    /// Pass values written to this stream to the supplied operator for transformation and write them to a target stream
    /// </summary>
    /// <typeparam name="TTransformedValue">Type of the value after transformation</typeparam>
    /// <param name="op">Operator that will write values to the target stream</param>
    /// <returns>The target stream the supplied operator will write transformed values to</returns>
    IOperandStream<TTransformedValue> Pipe<TTransformedValue>(IOperator<TValue, TTransformedValue> op);
}
