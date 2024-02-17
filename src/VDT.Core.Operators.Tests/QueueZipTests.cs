using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class QueueZipTests {
    [Fact]
    public async Task DoesNotPublishWithoutValue() {
        var additionalStream = new OperandStream<string>();
        var subject = new QueueZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await additionalStream.Publish("Foo", cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Any<(string, string)>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DoesNotPublishWithoutAdditionalValue() {
        var additionalStream = new OperandStream<string>();
        var subject = new QueueZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Any<(string, string)>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesWhenReceivingValueSecond() {
        var additionalStream = new OperandStream<string>();
        var subject = new QueueZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await additionalStream.Publish("Bar", cancellationTokenSource.Token);
        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish(("Foo", "Bar"), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task PublishesWhenReceivingAdditionalValueSecond() {
        var additionalStream = new OperandStream<string>();
        var subject = new QueueZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        await additionalStream.Publish("Bar", cancellationTokenSource.Token);

        await targetStream.Received().Publish(("Foo", "Bar"), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task EnqueuesOldValues() {
        var additionalStream = new OperandStream<string>();
        var subject = new QueueZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await additionalStream.Publish("Qux", cancellationTokenSource.Token);
        await additionalStream.Publish("Quux", cancellationTokenSource.Token);

        await targetStream.Received().Publish(("Foo", "Qux"), cancellationTokenSource.Token);
        await targetStream.Received().Publish(("Bar", "Quux"), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task EnqueuesOldAdditionalValues() {
        var additionalStream = new OperandStream<string>();
        var subject = new QueueZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await additionalStream.Publish("Foo", cancellationTokenSource.Token);
        await additionalStream.Publish("Bar", cancellationTokenSource.Token);
        await additionalStream.Publish("Baz", cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Quux", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish(("Qux", "Foo"), cancellationTokenSource.Token);
        await targetStream.Received().Publish(("Quux", "Bar"), cancellationTokenSource.Token);
    }
}
