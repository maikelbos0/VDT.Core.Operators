using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that pauses at least the specified delay in between publishing values, discarding older values when a new value is received while awaiting the delay
/// </summary>
/// <typeparam name="TValue">Type of value to throttle</typeparam>
public class Throttle<TValue> : IOperator<TValue, TValue> {
    private readonly Func<CancellationToken, Task<int>> delayFunc;
    private int operationId = 0;
    private DateTime nextPublishTime = DateTime.MinValue;

    internal Func<int, CancellationToken, Task> Delay { get; set; } = Task.Delay;
    internal Func<DateTime> UtcNow { get; set; } = () => DateTime.UtcNow;

    /// <summary>
    /// Create a throttle operator
    /// </summary>
    /// <param name="delayInMilliseconds">Time in milliseconds to wait in between publishing values</param>
    public Throttle(int delayInMilliseconds)
        : this(_ => Task.FromResult(delayInMilliseconds)) { }

    /// <summary>
    /// Create a throttle operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    public Throttle(Func<int> delayFunc)
        : this(_ => Task.FromResult(delayFunc())) { }

    /// <summary>
    /// Create a throttle operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    public Throttle(Func<Task<int>> delayFunc)
        : this(_ => delayFunc()) { }

    /// <summary>
    /// Create a throttle operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait in between publishing values</param>
    public Throttle(Func<CancellationToken, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    /// <inheritdoc/>
    public async Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        var now = UtcNow();
        var requiredDelayInMilliseconds = (nextPublishTime - now).TotalMilliseconds;

        if (requiredDelayInMilliseconds > 0) {
            var expectedOperationId = Interlocked.Increment(ref operationId);

            await Delay((int)requiredDelayInMilliseconds, cancellationToken);

            if (operationId != expectedOperationId) {
                return;
            }
        }

        nextPublishTime = UtcNow().AddMilliseconds(await delayFunc(cancellationToken));

        await targetStream.Publish(value, cancellationToken);
    }
}
