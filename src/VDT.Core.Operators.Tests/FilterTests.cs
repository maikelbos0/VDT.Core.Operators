using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class FilterTests {
    [Fact]
    public async Task DoesNotWriteWhenNotMatched() {
        var subject = new Filter<string>(value => value.StartsWith('B'));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Write(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WritesWhenMatched() {
        var subject = new Filter<string>(value => value.StartsWith('B'));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Write("Bar", cancellationTokenSource.Token);
    }
}
