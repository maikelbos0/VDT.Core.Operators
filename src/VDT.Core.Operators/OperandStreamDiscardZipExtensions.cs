namespace VDT.Core.Operators;

public static class OperandStreamDiscardZipExtensions {
    public static IOperandStream<(TValue, TAdditionalValue)> DiscardZip<TValue, TAdditionalValue>(this IOperandStream<TValue> operandStream, IOperandStream<TAdditionalValue> additionalStream)
        => operandStream.Pipe(new DiscardZip<TValue, TAdditionalValue>(), additionalStream);
}
