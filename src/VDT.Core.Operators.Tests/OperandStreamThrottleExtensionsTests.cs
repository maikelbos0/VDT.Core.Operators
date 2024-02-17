using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamThrottleExtensionsTests {
    [Fact]
    public void Throttle_Constant() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Throttle(500);

        subject.Received().Pipe(Arg.Any<Throttle<string>>());
    }

    [Fact]
    public void Throttle_Function() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Throttle(() => 500);

        subject.Received().Pipe(Arg.Any<Throttle<string>>());
    }

    [Fact]
    public void Throttle_TaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Throttle(() => Task.FromResult(500));

        subject.Received().Pipe(Arg.Any<Throttle<string>>());
    }

    [Fact]
    public void Throttle_CancellableTaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Throttle(_ => Task.FromResult(500));

        subject.Received().Pipe(Arg.Any<Throttle<string>>());
    }
}
