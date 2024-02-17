namespace VDT.Core.Operators;

public static class OperandStreamQueueZipExtensions {
    public static IOperandStream<(TValue, TAdditionalValue)> QueueZip<TValue, TAdditionalValue>(this IOperandStream<TValue> operandStream, IOperandStream<TAdditionalValue> additionalStream)
        => operandStream.Pipe(new QueueZip<TValue, TAdditionalValue>(), additionalStream);
}
