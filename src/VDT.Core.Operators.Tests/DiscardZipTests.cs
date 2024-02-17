using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class DiscardZipTests {
    [Fact]
    public async Task DoesNotPublishWithoutValue() {
        var additionalStream = new OperandStream<string>();
        var subject = new DiscardZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await additionalStream.Publish("Foo", cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Any<(string, string)>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DoesNotPublishWithoutAdditionalValue() {
        var additionalStream = new OperandStream<string>();
        var subject = new DiscardZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Any<(string, string)>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesWhenReceivingValueSecond() {
        var additionalStream = new OperandStream<string>();
        var subject = new DiscardZip<string, string>();
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
        var subject = new DiscardZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        await additionalStream.Publish("Bar", cancellationTokenSource.Token);

        await targetStream.Received().Publish(("Foo", "Bar"), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task DiscardsOldValue() {
        var additionalStream = new OperandStream<string>();
        var subject = new DiscardZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await subject.Execute("No", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        await additionalStream.Publish("Bar", cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Is<(string Value, string AdditionalValue)>(x => x.Value == "No"), Arg.Any<CancellationToken>());
        await targetStream.Received().Publish(("Foo", "Bar"), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task DiscardsOldAdditionalValue() {
        var additionalStream = new OperandStream<string>();
        var subject = new DiscardZip<string, string>();
        var targetStream = Substitute.For<IOperandStream<(string, string)>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Initialize(targetStream, additionalStream);

        await additionalStream.Publish("No", cancellationTokenSource.Token);
        await additionalStream.Publish("Bar", cancellationTokenSource.Token);
        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Is<(string Value, string AdditionalValue)>(x => x.AdditionalValue == "No"), Arg.Any<CancellationToken>());
        await targetStream.Received().Publish(("Foo", "Bar"), cancellationTokenSource.Token);
    }
}
