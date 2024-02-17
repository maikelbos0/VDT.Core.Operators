using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamDiscardZipExtensionsTests {
    [Fact]
    public void DiscardZip() {
        var subject = Substitute.For<IOperandStream<string>>();
        var additionalStream = Substitute.For<IOperandStream<string>>();

        _ = subject.DiscardZip(additionalStream);

        subject.Received().Pipe(Arg.Any<DiscardZip<string, string>>(), additionalStream);
    }
}
