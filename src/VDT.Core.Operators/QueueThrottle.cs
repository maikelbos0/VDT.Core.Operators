using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class QueueThrottle<TValue> : IOperator<TValue, TValue> {
    private readonly Func<CancellationToken, Task<int>> delayFunc;
    private readonly object nextPublishTimeLock = new();
    private DateTime nextPublishTime = DateTime.MinValue;

    internal Func<int, CancellationToken, Task> Delay { get; set; } = Task.Delay;
    internal Func<DateTime> UtcNow { get; set; } = () => DateTime.UtcNow;

    public QueueThrottle(int delayInMilliseconds)
        : this(_ => Task.FromResult(delayInMilliseconds)) { }

    public QueueThrottle(Func<int> delayFunc)
        : this(_ => Task.FromResult(delayFunc())) { }

    public QueueThrottle(Func<Task<int>> delayFunc)
        : this(_ => delayFunc()) { }

    public QueueThrottle(Func<CancellationToken, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        var now = UtcNow();
        var delayInMilliseconds = await delayFunc(cancellationToken);
        double requiredDelayInMilliseconds;

        lock (nextPublishTimeLock) {
            requiredDelayInMilliseconds = (nextPublishTime - now).TotalMilliseconds;

            if (requiredDelayInMilliseconds > 0) {
                nextPublishTime = nextPublishTime.AddMilliseconds(delayInMilliseconds);
            }
            else {
                nextPublishTime = now.AddMilliseconds(delayInMilliseconds);
            }
        }

        if (requiredDelayInMilliseconds > 0) {
            await Delay((int)requiredDelayInMilliseconds, cancellationToken);
        }

        await targetStream.Publish(value, cancellationToken);
    }
}
