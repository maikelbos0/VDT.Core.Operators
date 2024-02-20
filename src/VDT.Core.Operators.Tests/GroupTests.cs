using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class GroupTests {
    [Fact]
    public async Task GroupsByKeySelector_Function() {
        var subject = new Group<string, char>(value => value.FirstOrDefault());
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Foo" })), cancellationTokenSource.Token);
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task GroupsByKeySelector_TaskFunction() {
        var subject = new Group<string, char>(value => Task.FromResult(value.FirstOrDefault()));
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Foo" })), cancellationTokenSource.Token);
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }

    [Fact]
    public async Task GroupsByKeySelector_CancellableTaskFunction() {
        var func = Substitute.For<Func<string, CancellationToken, Task<char>>>();
        var subject = new Group<string, char>(func);
        var targetStream = Substitute.For<IOperandStream<List<string>>>();
        var cancellationTokenSource = new CancellationTokenSource();

        func.Invoke(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x => x.ArgAt<string>(0).FirstOrDefault());

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Bar", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Baz", targetStream, cancellationTokenSource.Token);
        await subject.Execute("Qux", targetStream, cancellationTokenSource.Token);

        await func.Received().Invoke(Arg.Any<string>(), cancellationTokenSource.Token);
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Foo" })), cancellationTokenSource.Token);
        await targetStream.Received().Publish(Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "Bar", "Baz" })), cancellationTokenSource.Token);
    }
}
