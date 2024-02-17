using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that publishes only those values to the target <see cref="IOperandStream{TValue}"/> that match the provided predicate
/// </summary>
/// <typeparam name="TValue">Type of value to filter</typeparam>
public class Filter<TValue> : IOperator<TValue, TValue> {
    private readonly Func<TValue, CancellationToken, Task<bool>> predicate;

    /// <summary>
    /// Create a filter operator
    /// </summary>
    /// <param name="predicate">Predicate against which values will be tested</param>
    public Filter(Func<TValue, bool> predicate)
        : this((value, _) => Task.FromResult(predicate(value))) { }

    /// <summary>
    /// Create a filter operator
    /// </summary>
    /// <param name="predicate">Predicate against which values will be tested</param>
    public Filter(Func<TValue, Task<bool>> predicate)
        : this((value, _) => predicate(value)) { }

    /// <summary>
    /// Create a filter operator
    /// </summary>
    /// <param name="predicate">Predicate against which values will be tested</param>
    public Filter(Func<TValue, CancellationToken, Task<bool>> predicate) {
        this.predicate = predicate;
    }

    /// <inheritdoc/>
    public async Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        if (await predicate(value, cancellationToken)) {
            await targetStream.Publish(value, cancellationToken);
        }
    }
}
