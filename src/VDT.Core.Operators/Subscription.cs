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
    /// Create a subscription
    /// </summary>
    /// <param name="operandStream">Operand stream to which this subscription belongs</param>
    public Subscription(IOperandStream<TValue> operandStream) {
        OperandStream = operandStream;
    }
}
