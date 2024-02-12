namespace VDT.Core.Operators;

// TODO add overloads
public static class OperandStreamDebounceExtensions {
    public static IOperandStream<TValue> Debounce<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Debounce<TValue>(delayInMilliseconds));
}
