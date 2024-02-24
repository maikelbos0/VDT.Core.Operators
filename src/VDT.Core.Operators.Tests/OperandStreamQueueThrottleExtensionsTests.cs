using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamQueueThrottleExtensionsTests {
    [Fact]
    public void QueueThrottle_Constant() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.QueueThrottle(500);

        subject.Received().Pipe(Arg.Any<QueueThrottle<string>>());
    }

    [Fact]
    public void QueueThrottle_Function() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.QueueThrottle(() => 500);

        subject.Received().Pipe(Arg.Any<QueueThrottle<string>>());
    }

    [Fact]
    public void QueueThrottle_TaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.QueueThrottle(() => Task.FromResult(500));

        subject.Received().Pipe(Arg.Any<QueueThrottle<string>>());
    }

    [Fact]
    public void QueueThrottle_CancellableTaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.QueueThrottle(_ => Task.FromResult(500));

        subject.Received().Pipe(Arg.Any<QueueThrottle<string>>());
    }
}
