using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that publishes transformed values received by a target <see cref="IOperandStream{TValue}"/> to a source <see cref="IOperandStream{TValue}"/>
/// </summary>
/// <typeparam name="TValue">Type of received values</typeparam>
/// <typeparam name="TTransformedValue">Type of published values</typeparam>
public interface IOperator<TValue, TTransformedValue> {
    /// <summary>
    /// Execute this operator
    /// </summary>
    /// <param name="value">Value received from the source <see cref="IOperandStream{TValue}"/></param>
    /// <param name="targetStream"><see cref="IOperandStream{TValue}"/> to publish values to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete</param>
    /// <returns></returns>
    Task Execute(TValue value, IOperandStream<TTransformedValue> targetStream, CancellationToken cancellationToken);
}

/// <inheritdoc/>
public interface IOperator<TValue, TTransformedValue, TInitializationValue> : IOperator<TValue, TTransformedValue> {
    /// <summary>
    /// Initialize the target <see cref="IOperandStream{TValue}"/>
    /// </summary>
    /// <param name="targetStream"><see cref="IOperandStream{TValue}"/> that values will be published to</param>
    /// <param name="initializationValue">Data to be used to initialize the <paramref name="targetStream"/></param>
    void Initialize(IOperandStream<TTransformedValue> targetStream, TInitializationValue initializationValue);

    // TODO rename initializationValue / TInitializationValue
}
