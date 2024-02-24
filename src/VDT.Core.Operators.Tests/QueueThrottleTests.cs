using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class QueueThrottleTests {
    [Fact]
    public async Task PublishesFirstValueImmediately() {
        var subject = new QueueThrottle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish("Foo", cancellationTokenSource.Token);
        await subject.Delay.DidNotReceive().Invoke(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesSecondValueAfterDelay_Constant() {
        var subject = new QueueThrottle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();
        subject.UtcNow = () => new DateTime(2024, 1, 2, 3, 14, 15, 30, DateTimeKind.Utc);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        Received.InOrder(() => {
            targetStream.Publish("Foo", cancellationTokenSource.Token);
            subject.Delay.Invoke(500, cancellationTokenSource.Token);
            targetStream.Publish("Bar", cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task PublishesSecondValueAfterDelay_Function() {
        var subject = new QueueThrottle<string>(() => 500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();
        subject.UtcNow = () => new DateTime(2024, 1, 2, 3, 14, 15, 30, DateTimeKind.Utc);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        Received.InOrder(() => {
            targetStream.Publish("Foo", cancellationTokenSource.Token);
            subject.Delay.Invoke(500, cancellationTokenSource.Token);
            targetStream.Publish("Bar", cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task PublishesSecondValueAfterDelay_TaskFunction() {
        var subject = new QueueThrottle<string>(() => Task.FromResult(500));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();
        subject.UtcNow = () => new DateTime(2024, 1, 2, 3, 14, 15, 30, DateTimeKind.Utc);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        Received.InOrder(() => {
            targetStream.Publish("Foo", cancellationTokenSource.Token);
            subject.Delay.Invoke(500, cancellationTokenSource.Token);
            targetStream.Publish("Bar", cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task PublishesSecondValueAfterDelay_CancellableTaskFunction() {
        var func = Substitute.For<Func<CancellationToken, Task<int>>>();
        var subject = new QueueThrottle<string>(func);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        func.Invoke(Arg.Any<CancellationToken>()).Returns(500);
        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();
        subject.UtcNow = () => new DateTime(2024, 1, 2, 3, 14, 15, 30, DateTimeKind.Utc);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        Received.InOrder(() => {
            func.Invoke(cancellationTokenSource.Token);
            targetStream.Publish("Foo", cancellationTokenSource.Token);
            func.Invoke(cancellationTokenSource.Token);
            subject.Delay.Invoke(500, cancellationTokenSource.Token);
            targetStream.Publish("Bar", cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task PublishesMoreValuesAfterMultipleDelays() {
        var subject = new QueueThrottle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();
        subject.UtcNow = () => new DateTime(2024, 1, 2, 3, 14, 15, 30, DateTimeKind.Utc);

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);

        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        Received.InOrder(() => {
            targetStream.Publish("Foo", cancellationTokenSource.Token);
            subject.Delay.Invoke(500, cancellationTokenSource.Token);
            targetStream.Publish("Bar", cancellationTokenSource.Token);
            subject.Delay.Invoke(1000, cancellationTokenSource.Token);
            targetStream.Publish("Baz", cancellationTokenSource.Token);
            subject.Delay.Invoke(1500, cancellationTokenSource.Token);
            targetStream.Publish("Qux", cancellationTokenSource.Token);
        });
    }
}
