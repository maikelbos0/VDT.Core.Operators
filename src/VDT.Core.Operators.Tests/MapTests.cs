using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class MapTests {
    [Fact]
    public async Task ReturnsMappedObject() {
        var subject = new Map<string, string>(value => $"{value}{value}");

        var result = await subject.Execute("Foo");

        Assert.True(result.IsAccepted);
        Assert.Equal("FooFoo", result.Value);
    }
}
