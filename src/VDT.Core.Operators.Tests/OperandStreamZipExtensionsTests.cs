using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamZipExtensionsTests {
    [Fact]
    public void Zip() {
        var subject = Substitute.For<IOperandStream<string>>();
        var additionalStream = Substitute.For<IOperandStream<string>>();

        _ = subject.Zip(additionalStream);

        subject.Received().Pipe(Arg.Any<Zip<string, string>>(), additionalStream);
    }
}
