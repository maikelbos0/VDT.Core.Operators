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
    /// Gets the task that represents the publishing of all initial values to this subscriber either because
    /// <see cref="OperandStreamOptions{TValue}.ReplayWhenSubscribing"/> is <see langword="true"/> or because 
    /// <see cref="OperandStreamOptions{TValue}.ValueGenerator"/> has a value
    /// </summary>
    public Task InitialPublishTask { get; private set; }

    internal Subscription(IOperandStream<TValue> operandStream, Task initialPublishTask) {
        OperandStream = operandStream;
        InitialPublishTask = initialPublishTask;
    }

    /// <summary>
    /// Unsubscribe the subscriber from the operand stream to which it belongs
    /// </summary>
    public void Unsubscribe() {
        OperandStream?.Unsubscribe(this);
    }
}
