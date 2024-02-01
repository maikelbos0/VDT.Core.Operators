using System;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperationResultTests {
    [Fact]
    public void Accepted() {
        var subject = OperationResult<string>.Accepted("Foo");

        Assert.True(subject.IsAccepted);
        Assert.Equal("Foo", subject.Value);
    }

    [Fact]
    public void Dismissed() {
        var subject = OperationResult<string>.Dismissed();

        Assert.False(subject.IsAccepted);
        Assert.Throws<InvalidOperationException>(() => subject.Value);
    }
}
