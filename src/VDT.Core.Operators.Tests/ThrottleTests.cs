﻿using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class ThrottleTests {
    [Fact]
    public async Task PublishesFirstValueImmediately() {
        var subject = new Throttle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish("Foo", cancellationTokenSource.Token);
        await subject.Delay.DidNotReceive().Invoke(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesSecondValueAfterDelay_Constant() {
        var subject = new Throttle<string>(500);
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
        var subject = new Throttle<string>(() => 500);
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
        var subject = new Throttle<string>(() => Task.FromResult(500));
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
        var subject = new Throttle<string>(func);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        func.Invoke(Arg.Any<CancellationToken>()).Returns(500);
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
    public async Task OnlyLastResultInIntervalIsPublished() {
        var isDelayed = true;
        var subject = new Throttle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = async (_, cancellationToken) => {
            while (isDelayed) {
                await Task.Delay(1, cancellationToken);
            }
        };

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        var task1 = subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        var task2 = subject.Execute("Baz", targetStream, cancellationTokenSource.Token);

        isDelayed = false;

        await Task.WhenAll(task1, task2);

        await targetStream.DidNotReceive().Publish("Bar", Arg.Any<CancellationToken>());
        await targetStream.Received().Publish("Baz", cancellationTokenSource.Token);
    }
}
