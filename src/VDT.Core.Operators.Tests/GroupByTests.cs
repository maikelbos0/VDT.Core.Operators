using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class GroupByTests {
    [Fact]
    public async Task GroupsByKeySelector_Function() {
        var subject = new GroupBy<string, char>(value => value.FirstOrDefault());
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task UsesEqualityComparer_Function() {
        var keyComparer = Substitute.For<IEqualityComparer<char>>();
        var subject = new GroupBy<string, char>(value => value.FirstOrDefault(), keyComparer);
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        keyComparer.Equals(Arg.Any<char>(), Arg.Any<char>()).Returns(c => c.ArgAt<char>(0) == c.ArgAt<char>(1));

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        keyComparer.Received().Equals('B', 'B');
        keyComparer.Received().Equals('B', 'Q');
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task GroupsByKeySelector_TaskFunction() {
        var subject = new GroupBy<string, char>(value => Task.FromResult(value.FirstOrDefault()));
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }
    
    [Fact]
    public async Task UsesEqualityComparer_TaskFunction() {
        var keyComparer = Substitute.For<IEqualityComparer<char>>();
        var subject = new GroupBy<string, char>(value => Task.FromResult(value.FirstOrDefault()), keyComparer);
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        keyComparer.Equals(Arg.Any<char>(), Arg.Any<char>()).Returns(c => c.ArgAt<char>(0) == c.ArgAt<char>(1));

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        keyComparer.Received().Equals('B', 'B');
        keyComparer.Received().Equals('B', 'Q');
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task GroupsByKeySelector_CancellableTaskFunction() {
        var func = Substitute.For<Func<string, CancellationToken, Task<char>>>();
        var subject = new GroupBy<string, char>(func);
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        func.Invoke(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<string>(0).FirstOrDefault());

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        await func.Received().Invoke(Arg.Any<string>(), cancellationTokenSource.Token);
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task UsesEqualityComparer_CancellableTaskFunction() {
        var func = Substitute.For<Func<string, CancellationToken, Task<char>>>();
        var keyComparer = Substitute.For<IEqualityComparer<char>>();
        var subject = new GroupBy<string, char>(func, keyComparer);
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        func.Invoke(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<string>(0).FirstOrDefault());
        keyComparer.Equals(Arg.Any<char>(), Arg.Any<char>()).Returns(c => c.ArgAt<char>(0) == c.ArgAt<char>(1));

        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        await func.Received().Invoke(Arg.Any<string>(), cancellationTokenSource.Token);
        keyComparer.Received().Equals('B', 'B');
        keyComparer.Received().Equals('B', 'Q');
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }
}
