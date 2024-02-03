using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Flatten<TValue> : IOperator<IOperandStream<TValue>, TValue> {
    public Task Execute(IOperandStream<TValue> value, IOperandStream<TValue> targetStream) {
        value.Subscribe(targetStream.Write);

        return Task.CompletedTask;
    }
}
