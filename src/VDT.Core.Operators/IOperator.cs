using System.Threading.Tasks;

namespace VDT.Core.Operators;

public interface IOperator<TValue, TNewValue> {
    IOperandStream<TNewValue> GetResultStream() => new OperandStream<TNewValue>();

    Task<OperationResult<TNewValue>> Execute(TValue value);
}
