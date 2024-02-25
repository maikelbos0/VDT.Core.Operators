using System.Threading.Tasks;
using System.Threading;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently use a valueless <see cref="OperandStream"/>
/// </summary>
public static class OperandStreamExtensions {
    /// <summary>
    /// Publish to this stream
    /// </summary>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be published to</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once the publish action has been handled by all subscribers and operators</returns>
    public static Task Publish(this IOperandStream<Void> operandStream)
        => operandStream.Publish(new Void());

    /// <summary>
    /// Publish to this stream
    /// </summary>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be published to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete</param>
    /// <returns>A <see cref="Task"/> which completes asynchronously once the publish action has been handled by all subscribers and operators</returns>
    public static Task Publish(this IOperandStream<Void> operandStream, CancellationToken cancellationToken)
        => operandStream.Publish(new Void(), cancellationToken);
}
