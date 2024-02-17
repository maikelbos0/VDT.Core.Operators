using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamMapExtensionsTests {
    [Fact]
    public void Map_Function() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Map(value => $"{value}{value}");

        subject.Received().Pipe(Arg.Any<Map<string, string>>());
    }

    [Fact]
    public void Map_TaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Map(value => Task.FromResult($"{value}{value}"));

        subject.Received().Pipe(Arg.Any<Map<string, string>>());
    }

    [Fact]
    public void Map_CancellableTaskFunction() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Map((value, _) => Task.FromResult($"{value}{value}"));

        subject.Received().Pipe(Arg.Any<Map<string, string>>());
    }
}
