using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class FlattenTests {
    [Fact]
    public async Task WritesInnerStreamValues() {
        var subject = new Flatten<string>();
        var targetStream = Substitute.For<IOperandStream<string>>();

        for (var i = 0; i < 2; i++) {
            var valueStream = new OperandStream<string>();

            await subject.Execute(valueStream, targetStream);

            await valueStream.Write($"Foo {i}");
            await valueStream.Write($"Bar {i}");
        }

        await targetStream.Received().Write("Foo 0");
        await targetStream.Received().Write("Foo 1");
        await targetStream.Received().Write("Bar 0");
        await targetStream.Received().Write("Bar 1");
    }
}
