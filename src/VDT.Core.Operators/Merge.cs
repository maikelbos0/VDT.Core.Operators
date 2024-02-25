using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
///  Operator that publishes values received from two or more instances of <see cref="IOperandStream{TValue}"/> of the same <typeparamref name="TValue"/>
/// </summary>
/// <typeparam name="TValue">Type of values</typeparam>
public class Merge<TValue> : IOperator<TValue, TValue, IEnumerable<IOperandStream<TValue>>> {
    /// <inheritdoc/>
    public void Initialize(IOperandStream<TValue> targetStream, IEnumerable<IOperandStream<TValue>> initializationData) {
        var subscriber = (Func<TValue, CancellationToken, Task>)targetStream.Publish;

        foreach (var operandStream in initializationData) {
            operandStream.Subscribe(subscriber);
        }
    }

    /// <inheritdoc/>
    public Task Execute(TValue value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        return targetStream.Publish(value, cancellationToken);
    }
}
