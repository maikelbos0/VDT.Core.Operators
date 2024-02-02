using NSubstitute;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamExtensionsTests {
    [Fact]
    public void Filter() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Filter(value => value.StartsWith('B'));

        subject.Received().Pipe(Arg.Any<Filter<string>>());
    }

    [Fact]
    public void Map() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Map(value => $"{value}{value}");

        subject.Received().Pipe(Arg.Any<Map<string, string>>());
    }

    [Fact]
    public void Debounce() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Debounce(500);

        subject.Received().Pipe(Arg.Any<Debounce<string>>());
    }
}
