using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamDebounceExtensionsTests {
    [Fact]
    public void Debounce() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Debounce(500);

        subject.Received().Pipe(Arg.Any<Debounce<string>>());
    }
}
