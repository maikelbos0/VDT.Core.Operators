﻿using System.Collections.Generic;
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

    [Fact]
    public async Task ReplayWhenSubscribingDoesNotSkipValues() {
        var subject = new OperandStream<int>(new OperandStreamOptions<int>() { ReplayWhenSubscribing = true });
        var cancellationTokenSource = new CancellationTokenSource();

        _ = PublishValuesContinuously(subject, cancellationTokenSource.Token);

        await Parallel.ForAsync(100, 200, async (i, cancellationToken) => {
            await Task.Delay(i, cancellationToken);

            var subscriber = new Subscriber();

            await subject.Subscribe(subscriber.ReceiveValue).InitialPublishTask;
            await Task.Delay(300 - i, cancellationToken);

            Assert.NotEmpty(subscriber.ReceivedValues);
            Assert.Equal(Enumerable.Range(0, subscriber.ReceivedValues.Count), subscriber.ReceivedValues);
        });

        cancellationTokenSource.Cancel();
    }
}
#endif
