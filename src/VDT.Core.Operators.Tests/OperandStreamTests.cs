using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamTests {
    [Fact]
    public async Task WritesValuesToSubscriber() {
        var subject = new OperandStream<string>();
        var subscribeAction = Substitute.For<Action<string>>();

        subject.Subscribe(subscribeAction);

        await subject.Write("Foo");
        await subject.Write("Bar");

        subscribeAction.Received().Invoke("Foo");
        subscribeAction.Received().Invoke("Bar");
    }

    [Fact]
    public async Task WritesValuesToMultipleSubscriber() {
        var subject = new OperandStream<string>();
        var subscribeAction = Substitute.For<Action<string>>();
        var subscribeFunc = Substitute.For<Func<string, Task>>();

        subject.Subscribe(subscribeAction);
        subject.Subscribe(subscribeFunc);

        await subject.Write("Foo");

        subscribeAction.Received().Invoke("Foo");
        await subscribeFunc.Received().Invoke("Foo");
    }

    [Fact]
    public async Task PipesValuesToOperator() {
        var subject = new OperandStream<string>();
        var op = Substitute.For<IOperator<string, string>>();
        var subscribeAction = Substitute.For<Action<string>>();

        op.Execute(Arg.Any<string>(), Arg.Any<IOperandStream<string>>()).Returns(callInfo => callInfo.ArgAt<IOperandStream<string>>(1).Write(callInfo.ArgAt<string>(0)));

        var result = subject.Pipe(op);

        result.Subscribe(subscribeAction);

        await subject.Write("Foo");

        subscribeAction.Received().Invoke("Foo");
    }
}
