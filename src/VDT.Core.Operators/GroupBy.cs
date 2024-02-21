using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class GroupBy<TValue, TKey> : IOperator<TValue, List<TValue>> {
    private readonly Func<TValue, CancellationToken, Task<TKey>> keySelector;
    private readonly IEqualityComparer<TKey> keyComparer;

    private readonly object groupLock = new();
    private TKey previousKey;
    private List<TValue> previousValues = new();

    public GroupBy(Func<TValue, TKey> keySelector)
        : this((value, _) => Task.FromResult(keySelector(value))) { }
    
    public GroupBy(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        : this((value, _) => Task.FromResult(keySelector(value)), keyComparer) { }

    public GroupBy(Func<TValue, Task<TKey>> keySelector)
        : this((value, _) => keySelector(value)) { }
    
    public GroupBy(Func<TValue, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer)
        : this((value, _) => keySelector(value), keyComparer) { }

    public GroupBy(Func<TValue, CancellationToken, Task<TKey>> keySelector) 
        : this(keySelector, EqualityComparer<TKey>.Default) { }

    public GroupBy(Func<TValue, CancellationToken, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer) {
        this.keySelector = keySelector;
        this.keyComparer = keyComparer;
    }

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
                previousValues = new() {
                    value
                };
            }
        }

        if (valuesToPublish.Count > 0) {
            await targetStream.Publish(valuesToPublish, cancellationToken);
        }
    }
}
