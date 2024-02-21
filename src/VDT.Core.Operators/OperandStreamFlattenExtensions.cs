namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamFlattenExtensions {
    /// <summary>
    /// Subscribe to an <see cref="IOperandStream{TValue}"/> of type <see cref="OperandStream{TValue}"/> and publish values of type <typeparamref name="TValue"/>
    /// </summary>
    /// <typeparam name="TValue">Type of operand stream to flatten</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that publishes values of type <typeparamref name="TValue"/></returns>
    public static IOperandStream<TValue> Flatten<TValue>(this IOperandStream<IOperandStream<TValue>> operandStream)
        => operandStream.Pipe(new Flatten<TValue>());
}
