using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Operator that publishes values transformed from <typeparamref name="TValue"/> into <typeparamref name="TTransformedValue"/> by the supplied transformation method
/// </summary>
/// <typeparam name="TValue">Type of received values</typeparam>
/// <typeparam name="TTransformedValue">Type of values to publish</typeparam>
public class Map<TValue, TTransformedValue> : IOperator<TValue, TTransformedValue> {
    private readonly Func<TValue, CancellationToken, Task<TTransformedValue>> func;

    /// <summary>
    /// Create a map operator
    /// </summary>
    /// <param name="func">Method that transforms received values into values to publish</param>
    public Map(Func<TValue, TTransformedValue> func)
        : this((value, _) => Task.FromResult(func(value))) { }

    /// <summary>
    /// Create a map operator
    /// </summary>
    /// <param name="func">Method that transforms received values into values to publish</param>
    public Map(Func<TValue, Task<TTransformedValue>> func)
        : this((value, _) => func(value)) { }

    /// <summary>
    /// Create a map operator
    /// </summary>
    /// <param name="func">Method that transforms received values into values to publish</param>
    public Map(Func<TValue, CancellationToken, Task<TTransformedValue>> func) {
        this.func = func;
    }

    /// <inheritdoc/>
    public async Task Execute(TValue value, IOperandStream<TTransformedValue> targetStream, CancellationToken cancellationToken)
        => await targetStream.Publish(await func(value, cancellationToken), cancellationToken);
}
