namespace VDT.Core.Operators;

// TODO add overloads
public static class OperandStreamThrottleExtensions {
    public static IOperandStream<TValue> Throttle<TValue>(this IOperandStream<TValue> operandStream, int delayInMilliseconds)
        => operandStream.Pipe(new Throttle<TValue>(delayInMilliseconds));
}
