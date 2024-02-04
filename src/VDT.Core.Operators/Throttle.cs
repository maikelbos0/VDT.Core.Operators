using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Throttle<TValue> : IOperator<TValue, TValue> {
    private readonly Func<TValue, Task<int>> delayFunc;
    private int operationId = 0;
    private DateTime nextExpectedExecutionTime = DateTime.MinValue;

    internal Func<int, CancellationToken, Task> Delay { get; set; } = Task.Delay;
    internal Func<DateTime> UtcNow { get; set; } = () => DateTime.UtcNow;

    public Throttle(int delayInMilliseconds)
        : this(value => Task.FromResult(delayInMilliseconds)) { }

    public Throttle(Func<TValue, int> delayFunc)
        : this(value => Task.FromResult(delayFunc(value))) { }

    public Throttle(Func<TValue, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        var now = UtcNow();
        var requiredDelayInMilliseconds = (nextExpectedExecutionTime - now).TotalMilliseconds;

        if (requiredDelayInMilliseconds > 0) {
            // Since Interlocked.Increment wraps, this will throttle properly until 2^32 operations occur in the delay
            var expectedOperationId = Interlocked.Increment(ref operationId);

            await Delay((int)requiredDelayInMilliseconds, cancellationToken);

            if (operationId != expectedOperationId) {
                return;
            }
        }

        nextExpectedExecutionTime = UtcNow().AddMilliseconds(await delayFunc(value));

        await targetStream.Write(value, cancellationToken);
    }
}
