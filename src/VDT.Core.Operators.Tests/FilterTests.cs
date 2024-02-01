using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class FilterTests {
    [Fact]
    public async Task ReturnsDismissedWhenNotMatched() {
        var subject = new Filter<string>(value => value.StartsWith('B'));

        var result = await subject.Execute("Foo");

        Assert.False(result.IsAccepted);
    }

    [Fact]
    public async Task ReturnsAcceptedWhenMatched() {
        var subject = new Filter<string>(value => value.StartsWith('B'));

        var result = await subject.Execute("Bar");

        Assert.True(result.IsAccepted);
    }
}
