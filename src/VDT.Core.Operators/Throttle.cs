using System;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Throttle<TValue> : IOperator<TValue, TValue> {
    // TODO extract to central place
    internal Func<int, Task> Delay { get; set; } = Task.Delay;

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
        // TODO extract DateTime.Now for mockability
        if (nextExpectedExecutionTime > DateTime.UtcNow) {
            var expectedExecutionTime = nextExpectedExecutionTime;

            await Delay((expectedExecutionTime - DateTime.UtcNow).Milliseconds);

            // TODO we may require a unique call identifier for high throughput
            if (nextExpectedExecutionTime != expectedExecutionTime) {
                return OperationResult<TValue>.Dismissed();
            }
        }

        // TODO we may require locking around nextExpectedExecutionTime
        nextExpectedExecutionTime = DateTime.UtcNow.AddMilliseconds(await delayFunc(value));

        return OperationResult<TValue>.Accepted(value);
    }
}
