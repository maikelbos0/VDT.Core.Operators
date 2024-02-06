using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Map<TValue, TTransformedValue> : IOperator<TValue, TTransformedValue> {
    private readonly Func<TValue, CancellationToken, Task<TTransformedValue>> func;

    public Map(Func<TValue, TTransformedValue> func)
        : this((value, _) => Task.FromResult(func(value))) { }

    public Map(Func<TValue, Task<TTransformedValue>> func)
        : this((value, _) => func(value)) { }

    public Map(Func<TValue, CancellationToken, Task<TTransformedValue>> func) { 
        this.func = func;
    }

    public async Task Execute(TValue value, IOperandStream<TTransformedValue> targetStream, CancellationToken cancellationToken)
        => await targetStream.Write(await func(value, cancellationToken), cancellationToken);
}
