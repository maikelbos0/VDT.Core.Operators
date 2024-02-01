using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

// TODO add cancellation tokens?
public class OperandStream<TValue> {
    private readonly List<Func<TValue, Task>> subscribers = new();

    public Task Write(TValue value) {
        return Task.WhenAll(subscribers.Select(subscriber => subscriber(value)));
    }

    public void Subscribe(Action<TValue> subscriber)
        => subscribers.Add(value => {
            subscriber(value);
            return Task.CompletedTask;
        });

    public void Subscribe(Func<TValue, Task> subscriber)
        => subscribers.Add(subscriber);
}
