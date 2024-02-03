using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Debounce<TValue> : IOperator<TValue, TValue> {
    // TODO extract to central place
    internal Func<int, Task> Delay { get; set; } = Task.Delay;

    private readonly Func<TValue, Task<int>> delayFunc;
    private int operationId = 0;

    public Debounce(int delayInMilliseconds)
        : this(value => Task.FromResult(delayInMilliseconds)) { }

    public Debounce(Func<TValue, int> delayFunc)
        : this(value => Task.FromResult(delayFunc(value))) { }

    public Debounce(Func<TValue, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task<OperationResult<TValue>> Execute(TValue value) {
        // Since Interlocked.Increment wraps, this will debounce properly until 2^32 operations occur in the delay
        var expectedOperationId = Interlocked.Increment(ref operationId);

        await Delay(await delayFunc(value));

        if (operationId == expectedOperationId) {
            return OperationResult<TValue>.Accepted(value);
        }
        else {
            return OperationResult<TValue>.Dismissed();
        }
    }
}
