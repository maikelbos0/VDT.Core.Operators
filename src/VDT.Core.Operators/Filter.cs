using System;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Filter<TValue> : IOperator<TValue, TValue> {
    private readonly Func<TValue, Task<bool>> predicate;

    public Filter(Func<TValue, bool> predicate)
        : this(value => Task.FromResult(predicate(value))) { }

    public Filter(Func<TValue, Task<bool>> predicate) {
        this.predicate = predicate;
    }

    public async Task Execute(TValue value, IOperandStream<TValue> targetStream) {
        if (await predicate(value)) {
            await targetStream.Write(value);
        }
    }
}
