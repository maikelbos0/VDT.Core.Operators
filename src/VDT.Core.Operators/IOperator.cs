using System.Threading.Tasks;

namespace VDT.Core.Operators;

public interface IOperator<TValue, TNewValue> {
    Task<OperationResult<TNewValue>> Execute(TValue value);
}
