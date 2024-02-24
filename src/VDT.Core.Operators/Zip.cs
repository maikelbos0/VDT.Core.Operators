using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that publishes tuples of the received values of two different <see cref="IOperandStream{TValue}"/> instances, discarding extra values received before a new tuple could be published
/// </summary>
/// <typeparam name="TValue">Type of received values from the first <see cref="IOperandStream{TValue}"/></typeparam>
/// <typeparam name="TAdditionalValue">Type of received values from the second <see cref="IOperandStream{TValue}"/></typeparam>
public class Zip<TValue, TAdditionalValue> : IOperator<TValue, (TValue, TAdditionalValue), IOperandStream<TAdditionalValue>> {
    private record class ValueReference<T>(T Value);

    private readonly object referenceLock = new();
    private ValueReference<TValue>? valueReference;
    private ValueReference<TAdditionalValue>? additionalValueReference;

    // TODO add option to use first or last value received when zipping

    /// <inheritdoc/>
    public void Initialize(IOperandStream<(TValue, TAdditionalValue)> targetStream, IOperandStream<TAdditionalValue> initializationData) {
        initializationData.Subscribe(async (additionalValue, cancellationToken) => {
            ValueReference<TValue>? valueReference = null;

            lock (referenceLock) {
                if (this.valueReference == null) {
                    additionalValueReference = new ValueReference<TAdditionalValue>(additionalValue);
                }
                else {
                    valueReference = this.valueReference;
                    this.valueReference = null;
                }
            }

            if (valueReference != null) {
                await targetStream.Publish((valueReference.Value, additionalValue), cancellationToken);
            }
        });
    }

    /// <inheritdoc/>
    public async Task Execute(TValue value, IOperandStream<(TValue, TAdditionalValue)> targetStream, CancellationToken cancellationToken) {
        ValueReference<TAdditionalValue>? additionalValueReference = null;

        lock (referenceLock) {
            if (this.additionalValueReference == null) {
                valueReference = new ValueReference<TValue>(value);
            }
            else {
                additionalValueReference = this.additionalValueReference;
                this.additionalValueReference = null;
            }
        }

        if (additionalValueReference != null) {
            await targetStream.Publish((value, additionalValueReference.Value), cancellationToken);
        }
    }
}
