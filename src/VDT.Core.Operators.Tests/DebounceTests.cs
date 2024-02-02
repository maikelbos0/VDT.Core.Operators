using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class DebounceTests {
    [Fact]
    public async Task DelaysForInterval() {
        var subject = new Debounce<string>(500);
        int? requestedDelay = null;

        subject.Delay = millisecondsDelay => {
            requestedDelay = millisecondsDelay;
            return Task.CompletedTask;
        };

        _ = await subject.Execute("Foo");

        Assert.Equal(500, requestedDelay);
    }

    [Fact]
    public async Task LastValueInIntervalIsAccepted() {
        var isDelayed = true;
        var subject = new Debounce<string>(500);

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        var task1 = subject.Execute("Foo");
        var task2 = subject.Execute("Bar");

        isDelayed = false;

        var result1 = await task1;
        var result2 = await task2;

        Assert.False(result1.IsAccepted);
        Assert.True(result2.IsAccepted);
    }

    [Fact]
    public async Task NewValueAfterIntervalIsAccepted() {
        var isDelayed = true;
        var subject = new Debounce<string>(500);

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        var tasks = new[] { subject.Execute("Foo"), subject.Execute("Bar") };

        isDelayed = false;

        await Task.WhenAll(tasks);

        var result = await subject.Execute("Baz");

        Assert.True(result.IsAccepted);
    }
}
