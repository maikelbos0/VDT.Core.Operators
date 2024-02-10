using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class FlattenTests {
    [Fact]
    public async Task PublishesInnerStreamValues() {
        var subject = new Flatten<string>();
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        for (var i = 0; i < 2; i++) {
            var valueStream = new OperandStream<string>();

            await subject.Execute(valueStream, targetStream, cancellationTokenSource.Token);

            await valueStream.Publish($"Foo {i}");
            await valueStream.Publish($"Bar {i}");
        }

        await targetStream.Received().Publish("Foo 0", CancellationToken.None);
        await targetStream.Received().Publish("Foo 1", CancellationToken.None);
        await targetStream.Received().Publish("Bar 0", CancellationToken.None);
        await targetStream.Received().Publish("Bar 1", CancellationToken.None);
    }
}
