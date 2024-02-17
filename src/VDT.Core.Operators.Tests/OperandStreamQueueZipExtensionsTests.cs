using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamQueueZipExtensionsTests {
    [Fact]
    public void QueueZip() {
        var subject = Substitute.For<IOperandStream<string>>();
        var additionalStream = Substitute.For<IOperandStream<string>>();

        _ = subject.QueueZip(additionalStream);

        subject.Received().Pipe(Arg.Any<QueueZip<string, string>>(), additionalStream);
    }
}
