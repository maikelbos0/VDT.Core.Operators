using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamDebounceExtensionsTests {
    [Fact]
    public void Debounce_Constant() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Debounce(500);

        subject.Received().Pipe(Arg.Any<Debounce<string>>());
    }

    [Fact]
    public void Debounce_Function() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Debounce(() => 500);

        subject.Received().Pipe(Arg.Any<Debounce<string>>());
    }

    [Fact]
    public void Debounce_TaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Debounce(() => Task.FromResult(500));

        subject.Received().Pipe(Arg.Any<Debounce<string>>());
    }

    [Fact]
    public void Debounce_CancellableTaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Debounce(_ => Task.FromResult(500));

        subject.Received().Pipe(Arg.Any<Debounce<string>>());
    }
}
