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
    /// Gets the task that represents the handling of any published values, including queued values if applicable
    /// </summary>
    public Task PublishTask { get; private set; }

    internal Subscription(IOperandStream<TValue> operandStream, Task initialPublishTask) {
        OperandStream = operandStream;
        PublishTask = initialPublishTask;
    }

    /// <summary>
    /// Unsubscribe the subscriber from the operand stream to which it belongs
    /// </summary>
    public void Unsubscribe() {
        OperandStream?.Unsubscribe(this);
    }
}
