using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamFilterExtensionsTests {
    [Fact]
    public void Filter() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Filter(value => value.StartsWith('B'));

        subject.Received().Pipe(Arg.Any<Filter<string>>());
    }
}
