using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamThrottleExtensionsTests {
    [Fact]
    public void Throttle() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Throttle(500);

        subject.Received().Pipe(Arg.Any<Throttle<string>>());
    }
}
