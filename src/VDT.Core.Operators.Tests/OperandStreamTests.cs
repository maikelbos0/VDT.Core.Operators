﻿using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamTests {
    [Fact]
    public async Task PublishesToSubscriberAction() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Action>();

        subject.Subscribe(subscriber);

        await subject.Publish("Foo");

        subscriber.Received().Invoke();
    }

    [Fact]
    public async Task PublishesValueToSubscriberAction() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Action<string>>();

        subject.Subscribe(subscriber);

        await subject.Publish("Foo");

        subscriber.Received().Invoke("Foo");
    }

    [Fact]
    public async Task PublishesToSubscriberTask() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Func<Task>>();

        subject.Subscribe(subscriber);

        await subject.Publish("Foo");

        await subscriber.Received().Invoke();
    }

    [Fact]
    public async Task PublishesValueToSubscriberTask() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Func<string, Task>>();

        subject.Subscribe(subscriber);

        await subject.Publish("Foo");

        await subscriber.Received().Invoke("Foo");
    }

    [Fact]
    public async Task PublishesToCancellableSubscriberTask() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Func<CancellationToken, Task>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Subscribe(subscriber);

        await subject.Publish("Foo", cancellationTokenSource.Token);

        await subscriber.Received().Invoke(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task PublishesValueToCancellableSubscriberTask() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Func<string, CancellationToken, Task>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Subscribe(subscriber);

        await subject.Publish("Foo", cancellationTokenSource.Token);

        await subscriber.Received().Invoke("Foo", cancellationTokenSource.Token);
    }

    [Fact]
    public async Task PublishesMultipleValuesToSubscriber() {
        var subject = new OperandStream<string>();
        var subscribeAction = Substitute.For<Action<string>>();

        subject.Subscribe(subscribeAction);

        await subject.Publish("Foo");
        await subject.Publish("Bar");

        subscribeAction.Received().Invoke("Foo");
        subscribeAction.Received().Invoke("Bar");
    }

    [Fact]
    public async Task PublishesValuesToMultipleSubscribers() {
        var subject = new OperandStream<string>();
        var subscriber1 = Substitute.For<Action<string>>();
        var subscriber2 = Substitute.For<Action<string>>();

        subject.Subscribe(subscriber1);
        subject.Subscribe(subscriber2);

        await subject.Publish("Foo");

        subscriber1.Received().Invoke("Foo");
        subscriber2.Received().Invoke("Foo");
    }

    [Fact]
    public async Task PipesValuesToOperator() {
        var subject = new OperandStream<string>();
        var op = Substitute.For<IOperator<string, string>>();
        var subscribeAction = Substitute.For<Action<string>>();

        op.Execute(Arg.Any<string>(), Arg.Any<IOperandStream<string>>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.ArgAt<IOperandStream<string>>(1).Publish(callInfo.ArgAt<string>(0)));

        var result = subject.Pipe(op);

        result.Subscribe(subscribeAction);

        await subject.Publish("Foo");

        subscribeAction.Received().Invoke("Foo");
    }

    [Fact]
    public async Task PipesValuesToOperatorWithInitialization() {
        var subject = new OperandStream<string>();
        var op = Substitute.For<IOperator<string, string, string>>();
        var subscribeAction = Substitute.For<Action<string>>();

        op.Execute(Arg.Any<string>(), Arg.Any<IOperandStream<string>>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.ArgAt<IOperandStream<string>>(1).Publish(callInfo.ArgAt<string>(0)));

        var result = subject.Pipe(op, "Bar");

        op.Received().Initialize(result, "Bar");

        result.Subscribe(subscribeAction);

        await subject.Publish("Foo");

        subscribeAction.Received().Invoke("Foo");
    }
}
