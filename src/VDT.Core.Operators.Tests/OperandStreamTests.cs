using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamTests {
    [Fact]
    public async Task WritesValueToSubscriberAction() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Action<string>>();

        subject.Subscribe(subscriber);

        await subject.Write("Foo");

        subscriber.Received().Invoke("Foo");
    }

    [Fact]
    public async Task WritesValueToSubscriberTask() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Func<string, Task>>();

        subject.Subscribe(subscriber);

        await subject.Write("Foo");

        await subscriber.Received().Invoke("Foo");
    }

    [Fact]
    public async Task WritesValueToCancellableSubscriberTask() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Func<string, CancellationToken, Task>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Subscribe(subscriber);

        await subject.Write("Foo", cancellationTokenSource.Token);

        await subscriber.Received().Invoke("Foo", cancellationTokenSource.Token);
    }

    [Fact]
    public async Task WritesMultipleValuesToSubscriber() {
        var subject = new OperandStream<string>();
        var subscribeAction = Substitute.For<Action<string>>();

        subject.Subscribe(subscribeAction);

        await subject.Write("Foo");
        await subject.Write("Bar");

        subscribeAction.Received().Invoke("Foo");
        subscribeAction.Received().Invoke("Bar");
    }

    [Fact]
    public async Task WritesValuesToMultipleSubscribers() {
        var subject = new OperandStream<string>();
        var subscriber1 = Substitute.For<Action<string>>();
        var subscriber2 = Substitute.For<Action<string>>();

        subject.Subscribe(subscriber1);
        subject.Subscribe(subscriber2);

        await subject.Write("Foo");

        subscriber1.Received().Invoke("Foo");
        subscriber2.Received().Invoke("Foo");
    }

    [Fact]
    public async Task PipesValuesToOperator() {
        var subject = new OperandStream<string>();
        var op = Substitute.For<IOperator<string, string>>();
        var subscribeAction = Substitute.For<Action<string>>();

        op.Execute(Arg.Any<string>(), Arg.Any<IOperandStream<string>>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.ArgAt<IOperandStream<string>>(1).Write(callInfo.ArgAt<string>(0)));

        var result = subject.Pipe(op);

        result.Subscribe(subscribeAction);

        await subject.Write("Foo");

        subscribeAction.Received().Invoke("Foo");
    }
}
