using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

// TODO add initializing method and options somehow, so we can make Merge an operator
public interface IOperator<TValue, TTransformedValue> {
    Task Execute(TValue value, IOperandStream<TTransformedValue> targetStream, CancellationToken cancellationToken);
}
