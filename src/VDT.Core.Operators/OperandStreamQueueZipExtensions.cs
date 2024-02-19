namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamQueueZipExtensions {
    /// <summary>
    /// Publish tuples of the values received from <paramref name="operandStream"/> and <paramref name="additionalStream"/>, queueing values received until a new tuple could be published
    /// </summary>
    /// <typeparam name="TValue">Type of received values from <paramref name="operandStream"/></typeparam>
    /// <typeparam name="TAdditionalValue">Type of received values from <paramref name="additionalStream"/></typeparam>
    /// <param name="operandStream">First <see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="additionalStream">Second <see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that value tuples of (<typeparamref name="TValue"/>, <typeparamref name="TAdditionalValue"/>) will be published to</returns>
    public static IOperandStream<(TValue, TAdditionalValue)> QueueZip<TValue, TAdditionalValue>(this IOperandStream<TValue> operandStream, IOperandStream<TAdditionalValue> additionalStream)
        => operandStream.Pipe(new QueueZip<TValue, TAdditionalValue>(), additionalStream);
}
