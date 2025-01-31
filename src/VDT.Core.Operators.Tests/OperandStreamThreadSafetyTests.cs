using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

#if DEBUG
public class OperandStreamThreadSafetyTests {
    private class Subscriber {
        public List<int> ReceivedValues { get; set; } = [];


        public async Task ReceiveValue(int value, CancellationToken cancellationToken) {
            await Task.Delay(2, cancellationToken);
            ReceivedValues.Add(value);
        }
    }

    private static async Task PublishValuesContinuously(IOperandStream<int> operandStream, CancellationToken cancellationToken) {
        var value = 0;

        while (true) {
            await Task.Delay(1, cancellationToken);
            await operandStream.Publish(value++);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private static async IAsyncEnumerable<int> GenerateValues() {
        for (int i = -1; i >= -200; i--) {
            await Task.Delay(1);
            yield return i;
        }
    }

    [Fact]
    public async Task ReplayWhenSubscribingDoesNotSkipValues() {
        var subject = new OperandStream<int>(new OperandStreamOptions<int>() { ReplayWhenSubscribing = true });
        var cancellationTokenSource = new CancellationTokenSource();

        _ = PublishValuesContinuously(subject, cancellationTokenSource.Token);

        await Parallel.ForAsync(50, 100, async (i, cancellationToken) => {
            await Task.Delay(i, cancellationToken);

            var subscriber = new Subscriber();

            await subject.Subscribe(subscriber.ReceiveValue).PublishTask;
            await Task.Delay(50, cancellationToken);

            var receivedValues = subscriber.ReceivedValues.ToList();

            Assert.NotEmpty(receivedValues);
            Assert.Equal(Enumerable.Range(0, receivedValues.Count), receivedValues);
        });

        cancellationTokenSource.Cancel();
    }

    [Fact]
    public async Task ValueGeneratorDoesNotSkipValuesWithoutReplayValueGeneratorWhenSubscribing() {
        var subject = new OperandStream<int>(new OperandStreamOptions<int>() { ValueGenerator = GenerateValues, ReplayValueGeneratorWhenSubscribing = false, ReplayWhenSubscribing = true });
        var subscribers = new List<Subscriber>();

        await Parallel.ForAsync(50, 100, async (i, cancellationToken) => {
            await Task.Delay(i, cancellationToken);

            var subscriber = new Subscriber();

            subscribers.Add(subscriber);

            await subject.Subscribe(subscriber.ReceiveValue).PublishTask;
        });

        Assert.NotNull(subject.ValueGenerationTask);
        await subject.ValueGenerationTask;

        foreach (var subscriber in subscribers) {
            var receivedValues = subscriber.ReceivedValues.ToList();

            Assert.NotEmpty(receivedValues);
            Assert.Equal(Enumerable.Range(-200, 200).Reverse(), receivedValues);
        }
    }
}
#endif
