using System.Collections.Generic;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamExtensionsTests {
    [Fact]
    public void Filter() {
        var receivedValues = new List<string>();
        var subject = new OperandStream<string>();

        var result = subject.Filter(value => value.StartsWith('B'));
        result.Subscribe(receivedValues.Add);

        subject.Write("Foo");
        subject.Write("Bar");

        Assert.Equal(new[] { "Bar" }, receivedValues);
    }

    [Fact]
    public void Map() {
        var receivedValues = new List<string>();
        var subject = new OperandStream<string>();

        var result = subject.Map(value => $"{value}{value}");

        result.Subscribe(receivedValues.Add);

        subject.Write("Foo");
        Assert.Equal(new[] { "FooFoo" }, receivedValues);
    }
}
