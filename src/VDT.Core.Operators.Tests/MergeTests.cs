using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class MergeTests {
    [Fact]
    public async Task MergesMultipleStreams() {
        var subject = new Merge<string>();
        var targetStream = Substitute.For<IOperandStream<string>>();
        var additionalStream1 = new OperandStream<string>();
        var additionalStream2 = new OperandStream<string>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, [additionalStream1, additionalStream2]);

        await additionalStream1.Publish("Foo", cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await additionalStream2.Publish("Bar", cancellationTokenSource.Token);

        await targetStream.Received().Publish("Foo", cancellationTokenSource.Token);
        await targetStream.Received().Publish("Bar", cancellationTokenSource.Token);
        await targetStream.Received().Publish("Baz", cancellationTokenSource.Token);
    }
}
