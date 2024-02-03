using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class FilterTests {
    [Fact]
    public async Task DoesNotWriteWhenNotMatched() {
        var subject = new Filter<string>(value => value.StartsWith('B'));
        var targetStream = Substitute.For<IOperandStream<string>>();

        await subject.Execute("Foo", targetStream);

        await targetStream.DidNotReceive().Write(Arg.Any<string>());
    }

    [Fact]
    public async Task WritesWhenMatched() {
        var subject = new Filter<string>(value => value.StartsWith('B'));
        var targetStream = Substitute.For<IOperandStream<string>>();

        await subject.Execute("Bar", targetStream);

        await targetStream.Received().Write("Bar");
    }
}
