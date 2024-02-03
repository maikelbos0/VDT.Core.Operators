using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class MapTests {
    [Fact]
    public async Task WritesMappedObject() {
        var subject = new Map<string, string>(value => $"{value}{value}");
        var targetStream = Substitute.For<IOperandStream<string>>();

        await subject.Execute("Foo", targetStream);

        await targetStream.Received().Write("FooFoo");
    }
}
