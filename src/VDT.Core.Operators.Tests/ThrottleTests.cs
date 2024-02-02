using System;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class ThrottleTests {
    [Fact]
    public async Task FirstValueIsAccepted() {
        var subject = new Throttle<string>(500);

        subject.Delay = async millisecondsDelay => {
            while (true) {
                await Task.Delay(1);
            }
        };

        var result = await subject.Execute("Foo");

        Assert.True(result.IsAccepted);
    }

    [Fact]
    public async Task SecondValueIsAccepted() {
        var subject = new Throttle<string>(500);

        subject.Delay = millisecondsDelay => {
            return Task.CompletedTask;
        };

        _ = await subject.Execute("Foo");

        var result = await subject.Execute("Bar");

        Assert.True(result.IsAccepted);
    }

    [Fact]
    public async Task DelaysSecondValueForInterval() {
        var subject = new Throttle<string>(500);
        int? requestedDelay = null;

        subject.Delay = millisecondsDelay => {
            requestedDelay = millisecondsDelay;
            return Task.CompletedTask;
        };
        subject.UtcNow = () => new DateTime(2024, 1, 2, 3, 14, 15, 30, DateTimeKind.Utc);

        _ = await subject.Execute("Foo");

        Assert.Null(requestedDelay);

        _ = await subject.Execute("Bar");

        Assert.Equal(500, requestedDelay);
    }

    [Fact]
    public async Task LastResultInIntervalIsAccepted() {
        var isDelayed = true;
        var subject = new Throttle<string>(500);

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        _ = await subject.Execute("Foo");
        
        var task1 = subject.Execute("Bar");
        var task2 = subject.Execute("Baz");

        isDelayed = false;

        var result1 = await task1;
        var result2 = await task2;

        Assert.False(result1.IsAccepted);
        Assert.True(result2.IsAccepted);
    }
}
