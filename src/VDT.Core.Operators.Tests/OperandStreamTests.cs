using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamTests {
    // TODO could we substitute funcs?
    [Fact]
    public async Task WritesValuesToSubscriber() {
        var receivedValues = new List<string>();

        var subject = new OperandStream<string>();

        subject.Subscribe(receivedValues.Add);

        await subject.Write("Foo");
        await subject.Write("Bar");

        Assert.Equal(new[] { "Foo", "Bar" }, receivedValues);
    }

    [Fact]
    public async Task WritesValuesToMultipleSubscriber() {
        var receivedValues = new List<string>();

        var subject = new OperandStream<string>();

        subject.Subscribe(receivedValues.Add);
        subject.Subscribe(async value => {
            await Task.Delay(1);
            receivedValues.Add(value);
        });

        await subject.Write("Foo");

        Assert.Equal(new[] { "Foo", "Foo" }, receivedValues);
    }

    [Fact]
    public async Task PipesValuesToOperator() {
        var receivedValues = new List<string>();

        var subject = new OperandStream<string>();
        var op = Substitute.For<IOperator<string, string>>();

        op.Execute(Arg.Any<string>(), Arg.Any<IOperandStream<string>>()).Returns(callInfo => callInfo.ArgAt<IOperandStream<string>>(1).Write(callInfo.ArgAt<string>(0)));

        var result = subject.Pipe(op);

        result.Subscribe(receivedValues.Add);

        await subject.Write("Foo");

        Assert.Equal(new[] { "Foo" }, receivedValues);
    }
}
