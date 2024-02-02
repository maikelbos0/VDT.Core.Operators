using System;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Throttle<TValue> : IOperator<TValue, TValue> {
    // TODO extract to central place
    internal Func<int, Task> Delay { get; set; } = Task.Delay;
    internal Func<DateTime> UtcNow { get; set; } = () => DateTime.UtcNow;

    private readonly Func<TValue, Task<int>> delayFunc;
    private DateTime nextExpectedExecutionTime = DateTime.MinValue;

    public Throttle(int delayInMilliseconds)
        : this(value => Task.FromResult(delayInMilliseconds)) { }

    public Throttle(Func<TValue, int> delayFunc)
        : this(value => Task.FromResult(delayFunc(value))) { }

    public Throttle(Func<TValue, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task<OperationResult<TValue>> Execute(TValue value) {
        var now = UtcNow();
        var requiredDelayInMilliseconds = (nextExpectedExecutionTime - now).TotalMilliseconds;

        if (requiredDelayInMilliseconds > 0) {
            var expectedExecutionTime = nextExpectedExecutionTime;

            await Delay((int)requiredDelayInMilliseconds);

            // TODO we may require a unique call identifier with locking for high throughput
            if (nextExpectedExecutionTime != expectedExecutionTime) {
                return OperationResult<TValue>.Dismissed();
            }
        }

        nextExpectedExecutionTime = UtcNow().AddMilliseconds(await delayFunc(value));

        return OperationResult<TValue>.Accepted(value);
    }
}
