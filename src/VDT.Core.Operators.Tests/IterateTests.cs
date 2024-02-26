using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class IterateTests {
    [Fact]
    public async Task Iterates() {
        var subject = new Iterate<char>();
        var targetStream = Substitute.For<IOperandStream<char>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        Received.InOrder(() => {
            targetStream.Publish('F', cancellationTokenSource.Token);
            targetStream.Publish('o', cancellationTokenSource.Token);
            targetStream.Publish('o', cancellationTokenSource.Token);
        });
    }
}
