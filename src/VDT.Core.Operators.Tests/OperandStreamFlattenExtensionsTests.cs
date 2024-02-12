using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamFlattenExtensionsTests {
    [Fact]
    public void Flatten() {
        var subject = Substitute.For<IOperandStream<IOperandStream<string>>>();

        _ = subject.Flatten();

        subject.Received().Pipe(Arg.Any<Flatten<string>>());
    }
}
