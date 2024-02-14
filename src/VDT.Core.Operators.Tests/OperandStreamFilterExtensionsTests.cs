using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamFilterExtensionsTests {
    [Fact]
    public void Filter_Predicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Filter(value => value.StartsWith('B'));

        subject.Received().Pipe(Arg.Any<Filter<string>>());
    }

    [Fact]
    public void Filter_TaskPredicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Filter(value => Task.FromResult(value.StartsWith('B')));

        subject.Received().Pipe(Arg.Any<Filter<string>>());
    }

    [Fact]
    public void Filter_CancellableTaskPredicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Filter((value, _) => Task.FromResult(value.StartsWith('B')));

        subject.Received().Pipe(Arg.Any<Filter<string>>());
    }
}
