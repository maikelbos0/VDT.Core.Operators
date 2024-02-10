using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamExtensionsTests {
    [Fact]
    public async Task WriteToVoidOperandStream() {
        var subject = Substitute.For<IOperandStream<Void>>();

        await subject.Write();

        await subject.Received().Write(Arg.Any<Void>());
    }

    [Fact]
    public async Task CancellableWriteToVoidOperandStream() {
        var cancellationTokenSource = new CancellationTokenSource();
        var subject = Substitute.For<IOperandStream<Void>>();

        await subject.Write(cancellationTokenSource.Token);

        await subject.Received().Write(Arg.Any<Void>(), cancellationTokenSource.Token);
    }

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

    [Fact]
    public void Throttle() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Throttle(500);

        subject.Received().Pipe(Arg.Any<Throttle<string>>());
    }

    [Fact]
    public void Flatten() {
        var subject = Substitute.For<IOperandStream<IOperandStream<string>>>();

        _ = subject.Flatten();

        subject.Received().Pipe(Arg.Any<Flatten<string>>());
    }
}
