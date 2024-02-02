using System;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Debounce<TValue> : IOperator<TValue, TValue> {
    // TODO extract to central place
    internal Func<int, Task> Delay { get; set; } = Task.Delay;

    private readonly Func<TValue, Task<int>> delayFunc;
    private DateTime lastExpectedExecutionTime = DateTime.MinValue;

    public Debounce(int delayInMilliseconds)
        : this(value => Task.FromResult(delayInMilliseconds)) { }

    public Debounce(Func<TValue, int> delayFunc)
        : this(value => Task.FromResult(delayFunc(value))) { }

    public Debounce(Func<TValue, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task<OperationResult<TValue>> Execute(TValue value) {
        var delayInMilliseconds = await delayFunc(value);

        // TODO we may require locking around lastExpectedExecutionTime
        // TODO extract DateTime.Now for mockability
        var expectedExecutionTime = lastExpectedExecutionTime = DateTime.UtcNow.AddMilliseconds(delayInMilliseconds);

        await Delay(delayInMilliseconds);

        // TODO we may require a unique call identifier for high throughput
        if (expectedExecutionTime == lastExpectedExecutionTime) {
            return OperationResult<TValue>.Accepted(value);
        }
        else {
            return OperationResult<TValue>.Dismissed();
        }
    }
}
