using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class DebounceTests {
    [Fact]
    public async Task DelaysForInterval() {
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = Substitute.For<Func<int, CancellationToken, Task>>();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await subject.Delay.Received().Invoke(500, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task WritesLastValueInInterval() {
        var isDelayed = true;
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        subject.Delay = async (_, _) => {
            while (isDelayed) {
                await Task.Delay(1);
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

        subject.Delay = async (_, _)  => {
            while (isDelayed) {
                await Task.Delay(1);
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
