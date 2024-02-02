using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class DebounceTests {
    [Fact]
    public async Task DelaysForInterval() {
        var subject = new Debounce<int>(500);
        int? requestedDelay = null;

        subject.Delay = millisecondsDelay => {
            requestedDelay = millisecondsDelay;
            return Task.CompletedTask;
        };

        await subject.Execute(1);

        Assert.Equal(500, requestedDelay);
    }

    [Fact]
    public async Task LastResultInIntervalIsAccepted() {
        var isDelayed = true;
        var subject = new Debounce<int>(500);

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        var task1 = subject.Execute(1);
        var task2 = subject.Execute(2);

        isDelayed = false;

        var result1 = await task1;
        var result2 = await task2;

        Assert.False(result1.IsAccepted);
        Assert.True(result2.IsAccepted);
    }

    [Fact]
    public async Task NewResultAfterIntervalIsAccepted() {
        var isDelayed = true;
        var subject = new Debounce<int>(500);

        subject.Delay = async millisecondsDelay => {
            while (isDelayed) {
                await Task.Delay(1);
            }
        };

        var task1 = subject.Execute(1);
        var task2 = subject.Execute(2);

        isDelayed = false;

        await Task.WhenAll(task1, task2);

        isDelayed = true;

        var task3 = subject.Execute(3);

        isDelayed = false;

        var value3 = await task3;

        Assert.True(value3.IsAccepted);
    }
}
