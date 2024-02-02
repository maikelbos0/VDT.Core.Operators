using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Flatten<TValue> : IOperator<IOperandStream<TValue>, TValue> {
    private readonly IOperandStream<TValue> flatStream = new OperandStream<TValue>();
    
    IOperandStream<TValue> IOperator<IOperandStream<TValue>, TValue>.GetResultStream() => flatStream;

    public Task<OperationResult<TValue>> Execute(IOperandStream<TValue> value) {
        value.Subscribe(flatStream.Write);

        return Task.FromResult(OperationResult<TValue>.Dismissed());
    }
}
