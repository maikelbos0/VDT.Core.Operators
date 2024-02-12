namespace VDT.Core.Operators;

// TODO add overloads
public static class OperandStreamFlattenExtensions {
    public static IOperandStream<TValue> Flatten<TValue>(this IOperandStream<IOperandStream<TValue>> operandStream)
        => operandStream.Pipe(new Flatten<TValue>());
}
