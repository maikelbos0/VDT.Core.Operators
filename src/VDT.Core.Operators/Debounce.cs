using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Debounce<TValue> : IOperator<TValue, TValue> {
    private readonly Func<CancellationToken, Task<int>> delayFunc;
    private int operationId = 0;

    internal Func<int, CancellationToken, Task> Delay { get; set; } = Task.Delay;

    public Debounce(int delayInMilliseconds)
        : this(_ => Task.FromResult(delayInMilliseconds)) { }

    public Debounce(Func<int> delayFunc)
        : this(_ => Task.FromResult(delayFunc())) { }

    public Debounce(Func<Task<int>> delayFunc)
        : this(_ => delayFunc()) { }

    public Debounce(Func<CancellationToken, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        // Since Interlocked.Increment wraps, this will debounce properly until 2^32 operations occur in the delay
        var expectedOperationId = Interlocked.Increment(ref operationId);

        await Delay(await delayFunc(cancellationToken), cancellationToken);

        if (operationId == expectedOperationId) {
            await targetStream.Publish(value, cancellationToken);
        }
    }
}
