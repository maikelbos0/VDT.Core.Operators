using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class FilterTests {
    [Fact]
    public async Task DoesNotPublishWhenNotMatched_Predicate() {
        var subject = new Filter<string>(value => value.StartsWith('B'));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesWhenMatched_Predicate() {
        var subject = new Filter<string>(value => value.StartsWith('B'));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish("Bar", cancellationTokenSource.Token);
    }

    [Fact]
    public async Task DoesNotPublishWhenNotMatched_TaskPredicate() {
        var subject = new Filter<string>(value => Task.FromResult(value.StartsWith('B')));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.DidNotReceive().Publish(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesWhenMatched_TaskPredicate() {
        var subject = new Filter<string>(value => Task.FromResult(value.StartsWith('B')));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish("Bar", cancellationTokenSource.Token);
    }

    [Fact]
    public async Task DoesNotPublishWhenNotMatched_CancellableTaskPredicate() {
        var predicate = Substitute.For<Func<string, CancellationToken, Task<bool>>>();
        var subject = new Filter<string>(predicate);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        predicate.Invoke(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.ArgAt<string>(0).StartsWith('B'));

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await predicate.Received().Invoke("Foo", cancellationTokenSource.Token);
        await targetStream.DidNotReceive().Publish(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishesWhenMatched_CancellableTaskPredicate() {
        var predicate = Substitute.For<Func<string, CancellationToken, Task<bool>>>();
        var subject = new Filter<string>(predicate);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        predicate.Invoke(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.ArgAt<string>(0).StartsWith('B'));

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        await predicate.Received().Invoke("Bar", cancellationTokenSource.Token);
        await targetStream.Received().Publish("Bar", cancellationTokenSource.Token);
    }
}
