using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class MapTests {
    [Fact]
    public async Task WritesMappedObject() {
        var subject = new Map<string, string>(value => $"{value}{value}");
        var targetStream = Substitute.For<IOperandStream<string>>();
        var cancellationTokenSource = new CancellationTokenSource();

        await subject.Execute("Foo", targetStream, cancellationTokenSource.Token);

        await targetStream.Received().Write("FooFoo", cancellationTokenSource.Token);
    }
}
