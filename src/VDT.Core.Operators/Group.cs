using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Group<TValue, TKey> : IOperator<TValue, List<TValue>> {
    private readonly Func<TValue, CancellationToken, Task<TKey>> keySelector;

    private readonly object groupLock = new();
    private TKey previousKey;
    private List<TValue> previousValues = new();

    public Group(Func<TValue, TKey> keySelector)
        : this((value, _) => Task.FromResult(keySelector(value))) { }

    public Group(Func<TValue, Task<TKey>> keySelector)
        : this((value, _) => keySelector(value)) { }

    public Group(Func<TValue, CancellationToken, Task<TKey>> keySelector) {
        this.keySelector = keySelector;
    }

    public async Task Execute(TValue value, IOperandStream<List<TValue>> targetStream, CancellationToken cancellationToken) {
        var key = await keySelector(value, cancellationToken);
        List<TValue>? valuesToPublish = null;

        lock (groupLock) {
            // TODO use supplied IEqualityComparer<TKey> ?? EqualityComparer<TKey>.Default
            if (Equals(previousKey, key)) {
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
