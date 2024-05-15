using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Represents a method subscribed to an <see cref="IOperandStream{TValue}"/>
/// </summary>
/// <typeparam name="TValue">Type of operand stream value</typeparam>
public sealed class Subscription<TValue> {
    /// <summary>
    /// Gets the operand stream to which this subscription belongs; returns <see langword="null"/> if unsubscribed
    /// </summary>
    public IOperandStream<TValue>? OperandStream { get; internal set; }

    /// <summary>
    /// Gets the method that gets executed when <see cref="OperandStream"/> publishes a value; returns <see langword="null"/> if unsubscribed
    /// </summary>
    public Func<TValue, CancellationToken, Task>? Subscriber { get; internal set; }

    /// <summary>
    /// Create a subscription
    /// </summary>
    /// <param name="subscriber">Method that gets executed when <paramref name="operandStream"/> publishes a value</param>
    /// <param name="operandStream">Operand stream to which this subscription belongs</param>
    public Subscription(IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task> subscriber) {
        OperandStream = operandStream;
        Subscriber = subscriber;
    }

    /// <summary>
    /// Unsubscribe the subscriber from the operand stream to which it belongs
    /// </summary>
    public void Unsubscribe() {
        if (OperandStream != null) {
            OperandStream.Unsubscribe(this);
        }
    }
}
