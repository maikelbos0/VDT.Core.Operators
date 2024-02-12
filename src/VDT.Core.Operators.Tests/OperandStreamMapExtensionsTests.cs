using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamMapExtensionsTests {
    [Fact]
    public void Map() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Map(value => $"{value}{value}");

        subject.Received().Pipe(Arg.Any<Map<string, string>>());
    }
}
