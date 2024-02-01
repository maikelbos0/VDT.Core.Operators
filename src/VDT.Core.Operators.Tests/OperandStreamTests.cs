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
        string? receivedValue1 = null;
        string? receivedValue2 = null;

        var subject = new OperandStream<string>();

        subject.Subscribe(value => receivedValue1 = value);
        subject.Subscribe(async value => {
            await Task.Delay(1);
            receivedValue2 = value;
        });

        await subject.Write("Foo");

        Assert.Equal("Foo", receivedValue1);
        Assert.Equal("Foo", receivedValue2);
    }
}
