using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class QueueZip<TValue, TAdditionalValue> : IOperator<TValue, (TValue, TAdditionalValue), IOperandStream<TAdditionalValue>> {
    private readonly object queueLock = new();
    private readonly Queue<TValue> values = new();
    private readonly Queue<TAdditionalValue> additionalValues = new();

    public void Initialize(IOperandStream<(TValue, TAdditionalValue)> targetStream, IOperandStream<TAdditionalValue> initializationData) {
        initializationData.Subscribe(async (additionalValue, cancellationToken) => {
            TValue? value = default;

            lock (queueLock) {
                if (!values.TryDequeue(out value)) {
                    additionalValues.Enqueue(additionalValue);
                    return;
                }
            }

            await targetStream.Publish((value, additionalValue), cancellationToken);
        });
    }

    public async Task Execute(TValue value, IOperandStream<(TValue, TAdditionalValue)> targetStream, CancellationToken cancellationToken) {
        TAdditionalValue? additionalValue = default;

        lock (queueLock) {
            if (!additionalValues.TryDequeue(out additionalValue)) {
                values.Enqueue(value);
                return;
            }
        }

        await targetStream.Publish((value, additionalValue), cancellationToken);
    }
}
