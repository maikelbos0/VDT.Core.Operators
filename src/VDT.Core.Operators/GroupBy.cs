using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that saves up values in a list, only publishing the previously saved values when the key changes
/// </summary>
/// <typeparam name="TValue">Type of value to group</typeparam>
/// <typeparam name="TKey">Type of the key</typeparam>
public class GroupBy<TValue, TKey> : IOperator<TValue, List<TValue>> {
    private readonly Func<TValue, CancellationToken, Task<TKey>> keySelector;
    private readonly IEqualityComparer<TKey> keyComparer;

    private readonly object groupLock = new();
    private TKey? previousKey;
    private List<TValue> previousValues = [];

    /// <summary>
    /// Create a group by operator
    /// </summary>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    public GroupBy(Func<TValue, TKey> keySelector)
        : this((value, _) => Task.FromResult(keySelector(value))) { }

    /// <summary>
    /// Create a group by operator
    /// </summary>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <param name="keyComparer"><see cref="IEqualityComparer{T}"/> to use when comparing the key</param>
    public GroupBy(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        : this((value, _) => Task.FromResult(keySelector(value)), keyComparer) { }

    /// <summary>
    /// Create a group by operator
    /// </summary>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    public GroupBy(Func<TValue, Task<TKey>> keySelector)
        : this((value, _) => keySelector(value)) { }

    /// <summary>
    /// Create a group by operator
    /// </summary>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <param name="keyComparer"><see cref="IEqualityComparer{T}"/> to use when comparing the key</param>
    public GroupBy(Func<TValue, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer)
        : this((value, _) => keySelector(value), keyComparer) { }

    /// <summary>
    /// Create a group by operator
    /// </summary>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    public GroupBy(Func<TValue, CancellationToken, Task<TKey>> keySelector) 
        : this(keySelector, EqualityComparer<TKey>.Default) { }

    /// <summary>
    /// Create a group by operator
    /// </summary>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <param name="keyComparer"><see cref="IEqualityComparer{T}"/> to use when comparing the key</param>
    public GroupBy(Func<TValue, CancellationToken, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer) {
        this.keySelector = keySelector;
        this.keyComparer = keyComparer;
    }

    /// <inheritdoc/>
    public async Task Execute(TValue value, IOperandStream<List<TValue>> targetStream, CancellationToken cancellationToken) {
        var key = await keySelector(value, cancellationToken);
        List<TValue>? valuesToPublish = null;

        lock (groupLock) {
            if (keyComparer.Equals(previousKey, key)) {
                previousValues.Add(value);
                return;
            }
            else {
                valuesToPublish = previousValues;
                previousKey = key;
                previousValues = [
                    value
                ];
            }
        }

        if (valuesToPublish.Count > 0) {
            await targetStream.Publish(valuesToPublish, cancellationToken);
        }
    }
}
