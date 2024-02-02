using System;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Debounce<TValue> : IOperator<TValue, TValue> {
    // TODO extract to central place
    internal Func<int, Task> Delay { get; set; } = Task.Delay;

    private readonly Func<TValue, Task<int>> delayFunc;
    private readonly object operationIdLock = new();
    private Guid operationId;

    public Debounce(int delayInMilliseconds)
        : this(value => Task.FromResult(delayInMilliseconds)) { }

    public Debounce(Func<TValue, int> delayFunc)
        : this(value => Task.FromResult(delayFunc(value))) { }

    public Debounce(Func<TValue, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task<OperationResult<TValue>> Execute(TValue value) {
        Guid expectedOperationId;
        bool isAccepted;

        lock (operationIdLock) {
            expectedOperationId = operationId = Guid.NewGuid();
        }

        await Delay(await delayFunc(value));

        lock (operationIdLock) {
            isAccepted = expectedOperationId == operationId;
        }

        if (isAccepted) {
            return OperationResult<TValue>.Accepted(value);
        }
        else {
            return OperationResult<TValue>.Dismissed();
        }
    }
}
