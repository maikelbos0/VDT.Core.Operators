using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that publishes only the last value received in succession after a specified delay, discarding those values received while the delay had not finished
/// </summary>
/// <typeparam name="TValue">Type of value to debounce</typeparam>
public class Debounce<TValue> : IOperator<TValue, TValue> {
    private readonly Func<CancellationToken, Task<int>> delayFunc;
    private int operationId = int.MaxValue - 1;

    internal Func<int, CancellationToken, Task> Delay { get; set; } = Task.Delay;

    /// <summary>
    /// Create a debounce operator
    /// </summary>
    /// <param name="delayInMilliseconds">Time in milliseconds to wait before publishing a value</param>
    public Debounce(int delayInMilliseconds)
        : this(_ => Task.FromResult(delayInMilliseconds)) { }

    /// <summary>
    /// Create a debounce operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait before publishing a value</param>
    public Debounce(Func<int> delayFunc)
        : this(_ => Task.FromResult(delayFunc())) { }

    /// <summary>
    /// Create a debounce operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait before publishing a value</param>
    public Debounce(Func<Task<int>> delayFunc)
        : this(_ => delayFunc()) { }

    /// <summary>
    /// Create a debounce operator
    /// </summary>
    /// <param name="delayFunc">Method that provides the time in milliseconds to wait before publishing a value</param>
    public Debounce(Func<CancellationToken, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    /// <inheritdoc/>
    public async Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        var expectedOperationId = Interlocked.Increment(ref operationId);

        await Delay(await delayFunc(cancellationToken), cancellationToken);

        if (operationId == expectedOperationId) {
            await targetStream.Publish(value, cancellationToken);
        }
    }
}
