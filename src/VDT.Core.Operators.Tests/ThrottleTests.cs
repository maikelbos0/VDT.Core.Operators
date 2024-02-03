using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class ThrottleTests {
    [Fact]
    public async Task WritesFirstValueImmediately() {
        int? requestedDelay = null;
        var subject = new Throttle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();

        subject.Delay = millisecondsDelay => {
            requestedDelay = millisecondsDelay;
            return Task.CompletedTask;
        };

        await subject.Execute("Foo", targetStream);

        await targetStream.Received().Write("Foo");
        Assert.Null(requestedDelay);
    }

    [Fact]
    public async Task WritesSecondValueAfterDelay() {
        int? requestedDelay = null;
        var subject = new Throttle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();

        subject.Delay = millisecondsDelay => {
            requestedDelay = millisecondsDelay;
            return Task.CompletedTask;
        };
        subject.UtcNow = () => new DateTime(2024, 1, 2, 3, 14, 15, 30, DateTimeKind.Utc);

        await subject.Execute("Foo", targetStream);

        await subject.Execute("Bar", targetStream);
        
        await targetStream.Received().Write("Bar");
        Assert.Equal(500, requestedDelay);
    }

    [Fact]
    public async Task OnlyLastResultInIntervalIsAccepted() {
        var isDelayed = true;
        var subject = new Throttle<string>(500);
        var targetStream = Substitute.For<IOperandStream<string>>();

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        await subject.Execute("Foo", targetStream);

        var task1 = subject.Execute("Bar", targetStream);
        var task2 = subject.Execute("Baz", targetStream);

        isDelayed = false;

        await Task.WhenAll(task1, task2);

        await targetStream.DidNotReceive().Write("Bar");
        await targetStream.Received().Write("Baz");
    }
}
