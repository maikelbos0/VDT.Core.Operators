using System.Threading.Tasks;
using System.Threading;

namespace VDT.Core.Operators;

public static class OperandStreamExtensions {
    public static Task Publish(this IOperandStream<Void> operandStream)
        => operandStream.Publish(new Void());

    public static Task Publish(this IOperandStream<Void> operandStream, CancellationToken cancellationToken)
        => operandStream.Publish(new Void(), cancellationToken);
}
