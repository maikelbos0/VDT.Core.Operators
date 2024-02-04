using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Map<TValue, TNewValue> : IOperator<TValue, TNewValue> {
    private readonly Func<TValue, Task<TNewValue>> func;

    public Map(Func<TValue, TNewValue> func)
        : this(value => Task.FromResult(func(value))) { }

    public Map(Func<TValue, Task<TNewValue>> func) {
        this.func = func;
    }

    public async Task Execute(TValue value, IOperandStream<TNewValue> targetStream, CancellationToken cancellationToken)
        => await targetStream.Write(await func(value), cancellationToken);
}
