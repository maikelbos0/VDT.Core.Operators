using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Filter<TValue> : IOperator<TValue, TValue> {
    private readonly Func<TValue, CancellationToken, Task<bool>> predicate;

    public Filter(Func<TValue, bool> predicate)
        : this((value, _) => Task.FromResult(predicate(value))) { }

    public Filter(Func<TValue, Task<bool>> predicate)
        : this((value, _) => predicate(value)) { }

    public Filter(Func<TValue, CancellationToken, Task<bool>> predicate) {
        this.predicate = predicate;
    }

    public async Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        if (await predicate(value, cancellationToken)) {
            await targetStream.Write(value, cancellationToken);
        }
    }
}
