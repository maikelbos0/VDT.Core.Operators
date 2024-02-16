using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public interface IOperator<TValue, TTransformedValue> {
    Task Execute(TValue value, IOperandStream<TTransformedValue> targetStream, CancellationToken cancellationToken);
}

public interface IOperator<TValue, TTransformedValue, TInitializationValue> {
    void Initialize(IOperandStream<TTransformedValue> targetStream, TInitializationValue initializationValue);

    Task Execute(TValue value, IOperandStream<TTransformedValue> targetStream, CancellationToken cancellationToken);
}
