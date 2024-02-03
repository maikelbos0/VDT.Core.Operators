using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class DebounceTests {
    [Fact]
    public async Task DelaysForInterval() {
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();
        int? requestedDelay = null;

        subject.Delay = millisecondsDelay => {
            requestedDelay = millisecondsDelay;
            return Task.CompletedTask;
        };

        await subject.Execute("Foo", targetStream);

        Assert.Equal(500, requestedDelay);
    }

    [Fact]
    public async Task WritesLastValueInInterval() {
        var isDelayed = true;
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        var task1 = subject.Execute("Foo", targetStream);
        var task2 = subject.Execute("Bar", targetStream);

        isDelayed = false;

        await Task.WhenAll(task1, task2);

        await targetStream.DidNotReceive().Write("Foo");
        await targetStream.Received().Write("Bar");
    }

    [Fact]
    public async Task WritesNewValueAfterInterval() {
        var isDelayed = true;
        var subject = new Debounce<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        var tasks = new[] { subject.Execute("Foo", targetStream), subject.Execute("Bar", targetStream) };

        isDelayed = false;

        await Task.WhenAll(tasks);

        await subject.Execute("Baz", targetStream);

        await targetStream.Received().Write("Baz");
    }
}
