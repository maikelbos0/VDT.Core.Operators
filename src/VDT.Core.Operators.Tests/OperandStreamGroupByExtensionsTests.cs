using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamGroupByByExtensionsTests {
    [Fact]
    public void GroupBy_Predicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.GroupBy(value => value.FirstOrDefault());

        subject.Received().Pipe(Arg.Any<GroupBy<string, char>>());
    }

    [Fact]
    public void GroupBy_Predicate_EqualityComparer() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.GroupBy(value => value.FirstOrDefault(), EqualityComparer<char>.Default);

        subject.Received().Pipe(Arg.Any<GroupBy<string, char>>());
    }

    [Fact]
    public void GroupBy_TaskPredicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.GroupBy(value => Task.FromResult(value.FirstOrDefault()));

        subject.Received().Pipe(Arg.Any<GroupBy<string, char>>());
    }

    [Fact]
    public void GroupBy_TaskPredicate_EqualityComparer() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.GroupBy(value => Task.FromResult(value.FirstOrDefault()), EqualityComparer<char>.Default);

        subject.Received().Pipe(Arg.Any<GroupBy<string, char>>());
    }

    [Fact]
    public void GroupBy_CancellableTaskPredicate() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.GroupBy((value, _) => Task.FromResult(value.FirstOrDefault()));

        subject.Received().Pipe(Arg.Any<GroupBy<string, char>>());
    }

    [Fact]
    public void GroupBy_CancellableTaskPredicate_EqualityComparer() {
        var subject = Substitute.For<IOperandStream<string>>();

        _ = subject.GroupBy((value, _) => Task.FromResult(value.FirstOrDefault()), EqualityComparer<char>.Default);

        subject.Received().Pipe(Arg.Any<GroupBy<string, char>>());
    }
}
