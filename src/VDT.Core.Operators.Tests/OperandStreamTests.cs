using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamTests {
    [Fact]
    public void ReturnsSubscription() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Func<string, CancellationToken, Task>>();

        var subscription = subject.Subscribe(subscriber);

        Assert.Equal(subject, subscription.OperandStream);
    }

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
        var subscriber = Substitute.For<Action<string>>();

        subject.Subscribe(subscriber);

        await subject.Publish("Foo");
        await subject.Publish("Bar");

        subscriber.Received().Invoke("Foo");
        subscriber.Received().Invoke("Bar");
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
    public async Task PublishesValuesToMultipleSubscriptionsOfSameSubscriber() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Action<string>>();

        subject.Subscribe(subscriber);
        subject.Subscribe(subscriber);

        await subject.Publish("Foo");

        subscriber.Received(2).Invoke("Foo");
    }

    //[Fact]
    //public async Task PublishesGeneratedValues() {
    //    var valueGenerator = Substitute.For<Func<IAsyncEnumerable<string>>>();
    //    valueGenerator.Invoke().Returns(new[] { "Foo", "Bar" }.ToAsyncEnumerable());

    //    var subject = new OperandStream<string>(new OperandStreamOptions<string>() { ValueGenerator = valueGenerator });
    //    var subscriber = Substitute.For<Func<string, CancellationToken, Task>>();

    //    var subscription = subject.Subscribe(subscriber);

    //    await subscription.InitialPublishTask;

    //    valueGenerator.Received().Invoke();
    //    await subscriber.Received().Invoke("Foo", CancellationToken.None);
    //    await subscriber.Received().Invoke("Bar", CancellationToken.None);
    //}

    //[Fact]
    //public async Task GeneratesValuesOnceIfReplayValueGeneratorWhenSubscribingIsDisabled() {
    //    var valueGenerator = Substitute.For<Func<IAsyncEnumerable<string>>>();
    //    valueGenerator.Invoke().Returns(new[] { "Foo", "Bar" }.ToAsyncEnumerable());

    //    var subject = new OperandStream<string>(new OperandStreamOptions<string>() { ValueGenerator = valueGenerator, ReplayValueGeneratorWhenSubscribing = false });
    //    var subscriber1 = Substitute.For<Func<string, CancellationToken, Task>>();
    //    var subscriber2 = Substitute.For<Func<string, CancellationToken, Task>>();

    //    var subscription1 = subject.Subscribe(subscriber1);
    //    var subscription2 = subject.Subscribe(subscriber2);

    //    await Task.WhenAll(subscription1.InitialPublishTask, subscription2.InitialPublishTask);

    //    valueGenerator.Received(1).Invoke();
    //    await subscriber1.Received().Invoke("Foo", CancellationToken.None);
    //    await subscriber1.Received().Invoke("Bar", CancellationToken.None);
    //    await subscriber2.Received().Invoke("Foo", CancellationToken.None);
    //    await subscriber2.Received().Invoke("Bar", CancellationToken.None);
    //}

    //[Fact]
    //public async Task GeneratesValuesForEverySubscriberIfReplayValueGeneratorWhenSubscribingIsEnabled() {
    //    var valueGenerator = Substitute.For<Func<IAsyncEnumerable<string>>>();
    //    valueGenerator.Invoke().Returns(new[] { "Foo", "Bar" }.ToAsyncEnumerable());

    //    var subject = new OperandStream<string>(new OperandStreamOptions<string>() { ValueGenerator = valueGenerator, ReplayValueGeneratorWhenSubscribing = true });
    //    var subscriber1 = Substitute.For<Func<string, CancellationToken, Task>>();
    //    var subscriber2 = Substitute.For<Func<string, CancellationToken, Task>>();

    //    var subscription1 = subject.Subscribe(subscriber1);
    //    var subscription2 = subject.Subscribe(subscriber2);

    //    await Task.WhenAll(subscription1.InitialPublishTask, subscription2.InitialPublishTask);

    //    valueGenerator.Received(2).Invoke();
    //    await subscriber1.Received().Invoke("Foo", CancellationToken.None);
    //    await subscriber1.Received().Invoke("Bar", CancellationToken.None);
    //    await subscriber2.Received().Invoke("Foo", CancellationToken.None);
    //    await subscriber2.Received().Invoke("Bar", CancellationToken.None);
    //}

    [Fact]
    public async Task PublishesPreviousValuesToNewSubscriberIfReplayWhenSubscribingIsDisabled() {
        var subject = new OperandStream<string>(new OperandStreamOptions<string>() { ReplayWhenSubscribing = false });
        var subscriber = Substitute.For<Func<string, CancellationToken, Task>>();

        await subject.Publish("Foo");

        var subscription = subject.Subscribe(subscriber);

        await subscription.PublishTask;

        await subscriber.DidNotReceive().Invoke(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesPreviousValuesToNewSubscriberIfReplayWhenSubscribingIsEnabled() {
        var subject = new OperandStream<string>(new OperandStreamOptions<string>() { ReplayWhenSubscribing = true });
        var subscriber = Substitute.For<Func<string, CancellationToken, Task>>();

        await subject.Publish("Foo");

        var subscription = subject.Subscribe(subscriber);

        await subscription.PublishTask;

        await subscriber.Received().Invoke("Foo", CancellationToken.None);
    }

    [Fact]
    public async Task PipesValuesToOperator() {
        var subject = new OperandStream<string>();
        var op = Substitute.For<IOperator<string, string>>();
        var subscriber = Substitute.For<Action<string>>();

        op.Execute(Arg.Any<string>(), Arg.Any<IOperandStream<string>>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.ArgAt<IOperandStream<string>>(1).Publish(callInfo.ArgAt<string>(0)));

        var result = subject.Pipe(op);

        result.Subscribe(subscriber);

        await subject.Publish("Foo");

        subscriber.Received().Invoke("Foo");
    }

    [Fact]
    public async Task PipesValuesToOperatorWithInitialization() {
        var subject = new OperandStream<string>();
        var op = Substitute.For<IOperator<string, string, string>>();
        var subscriber = Substitute.For<Action<string>>();

        op.Execute(Arg.Any<string>(), Arg.Any<IOperandStream<string>>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.ArgAt<IOperandStream<string>>(1).Publish(callInfo.ArgAt<string>(0)));

        var result = subject.Pipe(op, "Bar");

        op.Received().Initialize(result, "Bar");

        result.Subscribe(subscriber);

        await subject.Publish("Foo");

        subscriber.Received().Invoke("Foo");
    }

    [Fact]
    public async Task Unsubscribe() {
        var subject = new OperandStream<string>();
        var subscriber = Substitute.For<Action<string>>();
        var subscription = subject.Subscribe(subscriber);

        subject.Unsubscribe(subscription);

        await subject.Publish("Foo");

        subscriber.DidNotReceive().Invoke(Arg.Any<string>());
        Assert.Null(subscription.OperandStream);
    }

    [Fact]
    public void UnsubscribeDoesNotUnsubscribeForDifferentOperandStream() {
        var subject = new OperandStream<string>();
        var other = new OperandStream<string>();
        var subscriber = Substitute.For<Action<string>>();
        var subscription = other.Subscribe(subscriber);

        subject.Unsubscribe(subscription);

        Assert.NotNull(subscription.OperandStream);
    }

    [Fact]
    public async Task UnsubscribeAll() {
        var subject = new OperandStream<string>();
        var subscriber1 = Substitute.For<Action<string>>();
        var subscriber2 = Substitute.For<Action<string>>();

        var subscription1 = subject.Subscribe(subscriber1);
        var subscription2 = subject.Subscribe(subscriber2);

        subject.UnsubscribeAll();

        await subject.Publish("Foo");

        subscriber1.DidNotReceive().Invoke(Arg.Any<string>());
        subscriber2.DidNotReceive().Invoke(Arg.Any<string>());
        Assert.Null(subscription1.OperandStream);
        Assert.Null(subscription2.OperandStream);
    }
}
