using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class FlattenTests {
    [Fact]
    public async Task OutputsInnerStreamValues() {
        var receivedValues = new List<string>();

        var subject = new OperandStream<IOperandStream<string>>();

        var result = subject.Flatten();
        result.Subscribe(receivedValues.Add);

        for (var i = 0; i < 2; i++) {
            var valueStream = new OperandStream<string>();

            await subject.Write(valueStream);

            await valueStream.Write($"Foo {i}");
            await valueStream.Write($"Bar {i}");
        }

        Assert.Equal(new[] { "Foo 0", "Bar 0", "Foo 1", "Bar 1" }, receivedValues);
    }
}
