using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that iterates over received values of <see cref="IEnumerable{T}"/> of <typeparamref name="TValue"/> and publishes each item
/// </summary>
/// <typeparam name="TValue">Type of values</typeparam>
public class Iterate<TValue> : IOperator<IEnumerable<TValue>, TValue> {
    /// <inheritdoc/>
    public async Task Execute(IEnumerable<TValue> value, IOperandStream<TValue> targetStream, CancellationToken cancellationToken) {
        foreach (var item in value) {
            await targetStream.Publish(item, cancellationToken);
        }
    }
}
