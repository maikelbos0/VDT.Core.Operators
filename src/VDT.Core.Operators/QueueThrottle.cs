using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that pauses at least the specified delay in between publishing values, queueing received values
/// </summary>
/// <typeparam name="TValue">Type of value to throttle</typeparam>
public class QueueThrottle
    <TValue> : IOperator<TValue, TValue> {
    private readonly Func<CancellationToken, Task<int>> delayFunc;
    private readonly object nextPublishTimeLock = new();
    private DateTime nextPublishTime = DateTime.MinValue;

    internal Func<int, CancellationToken, Task> Delay { get; set; } = Task.Delay;
    internal Func<DateTime> UtcNow { get; set; } = () => DateTime.UtcNow;

    /// <summary>
    /// Create a queued throttle operator
    /// </summary>
    /// <param name="delayInMilliseconds">Time in milliseconds to wait in between publishing values</param>
    public QueueThrottle(int delayInMilliseconds)
        : this(_ => Task.FromResult(delayInMilliseconds)) { }

    /// <summary>
    /// Create a queued throttle operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    public QueueThrottle(Func<int> delayFunc)
        : this(_ => Task.FromResult(delayFunc())) { }

    /// <summary>
    /// Create a queued throttle operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    public QueueThrottle(Func<Task<int>> delayFunc)
        : this(_ => delayFunc()) { }

    /// <summary>
    /// Create a queued throttle operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    public QueueThrottle(Func<CancellationToken, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    /// <inheritdoc/>
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
