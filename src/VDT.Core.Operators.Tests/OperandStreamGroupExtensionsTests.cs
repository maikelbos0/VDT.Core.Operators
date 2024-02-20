using NSubstitute;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamGroupExtensionsTests {
    [Fact]
    public void Group_Predicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Group(value => value.FirstOrDefault());

        subject.Received().Pipe(Arg.Any<Group<string, char>>());
    }

    [Fact]
    public void Group_TaskPredicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Group(value => Task.FromResult(value.FirstOrDefault()));

        subject.Received().Pipe(Arg.Any<Group<string, char>>());
    }

    [Fact]
    public void Group_CancellableTaskPredicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.Group((value, _) => Task.FromResult(value.FirstOrDefault()));

        subject.Received().Pipe(Arg.Any<Group<string, char>>());
    }
}
