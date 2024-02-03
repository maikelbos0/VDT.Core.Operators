using System.Threading.Tasks;

namespace VDT.Core.Operators;

public interface IOperator<TValue, TNewValue> {
    Task Execute(TValue value, IOperandStream<TNewValue> targetStream);
}
