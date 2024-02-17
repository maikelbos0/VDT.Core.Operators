using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class MapTests {
    [Fact]
    public async Task PublishesMappedObject_Function() {
        var subject = new Map<string, string>(value => $"{value}{value}");
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish("FooFoo", cancellationTokenSource.Token);
    }

    [Fact]
    public async Task PublishesMappedObject_TaskFunction() {
        var subject = new Map<string, string>(value => Task.FromResult($"{value}{value}"));
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Publish("FooFoo", cancellationTokenSource.Token);
    }

    [Fact]
    public async Task PublishesMappedObject_CancellableTaskFunction() {
        var func = Substitute.For<Func<string, CancellationToken, Task<string>>>();
        var subject = new Map<string, string>(func);
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        func.Invoke(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(callInfo => $"{callInfo.ArgAt<string>(0)}{callInfo.ArgAt<string>(0)}");

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);
        
        await func.Received().Invoke("Foo", cancellationTokenSource.Token);
        await targetStream.Received().Publish("FooFoo", cancellationTokenSource.Token);
    }
}
