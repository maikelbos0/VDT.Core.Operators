using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamTests {
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

        op.GetResultStream().Returns(new OperandStream<string>());
        op.Execute("Foo").Returns(OperationResult<string>.Dismissed());
        op.Execute("Bar").Returns(OperationResult<string>.Accepted("Bar"));

        var result = subject.Pipe(op);

        result.Subscribe(receivedValues.Add);

        await subject.Write("Foo");
        await subject.Write("Bar");

        await op.Received().Execute("Foo");
        await op.Received().Execute("Bar");

        Assert.Equal(new[] { "Bar" }, receivedValues);
    }
}
