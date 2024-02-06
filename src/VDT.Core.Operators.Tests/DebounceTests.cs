using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class DebounceTests {
    [Fact]
    public async Task DelaysForInterval_Constant() {
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Delay.Received().Invoke(500, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task DelaysForInterval_Function() {
        var subject = new Debounce<string>(() => 500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Delay.Received().Invoke(500, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task DelaysForInterval_TaskFunction() {
        var subject = new Debounce<string>(() => Task.FromResult(500));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Delay.Received().Invoke(500, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task DelaysForInterval_CancellableTaskFunction() {
        var func = Substitute.For<Func<CancellationToken, Task<int>>>();
        var subject = new Debounce<string>(func);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        func.Invoke(Arg.Any<CancellationToken>()).Returns(500);

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await func.Received().Invoke(cancellationTokenSource.Token);
        await subject.Delay.Received().Invoke(500, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task WritesLastValueInInterval() {
        var isDelayed = true;
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = async (_, cancellationToken) => {
            while (isDelayed) {
                await Task.Delay(1, cancellationToken);
            }
        };

        var task1 = subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        var task2 = subject.Execute("Bar", targetStream, cancellationTokenSource.Token);

        isDelayed = false;

        await Task.WhenAll(task1, task2);

        await targetStream.DidNotReceive().Write("Foo", Arg.Any<CancellationToken>());
        await targetStream.Received().Write("Bar", cancellationTokenSource.Token);
    }

    [Fact]
    public async Task WritesNewValueAfterInterval() {
        var isDelayed = true;
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = async (_, cancellationToken)  => {
            while (isDelayed) {
                await Task.Delay(1, cancellationToken);
            }
        };

        var tasks = new[] {
            subject.Execute("Foo", targetStream, cancellationTokenSource.Token),
            subject.Execute("Bar", targetStream, cancellationTokenSource.Token)
        };

        isDelayed = false;

        await Task.WhenAll(tasks);

        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Write("Baz", cancellationTokenSource.Token);
    }
}
