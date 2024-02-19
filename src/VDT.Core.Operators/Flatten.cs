using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operation that subscribes to an <see cref="IOperandStream{TValue}"/> of type <see cref="OperandStream{TValue}"/> and publishes values of <typeparamref name="TValue"/>
/// </summary>
/// <typeparam name="TValue">Type of operand stream to flatten</typeparam>
public class Flatten<TValue> : IOperator<IOperandStream<TValue>, TValue> {
    /// <inheritdoc/>
    public Task Execute(IOperandStream<TValue> value, IOperandStream<TValue> targetStream, CancellationToken _) {
        value.Subscribe((Func<TValue, CancellationToken, Task>)targetStream.Publish);

        return Task.CompletedTask;
    }
}
