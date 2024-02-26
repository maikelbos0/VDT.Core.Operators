using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamIterateExtensionsTests {
    [Fact]
    public void Iterate() {
        var subject = Substitute.For<IOperandStream<IEnumerable<char>>>();

        _ = subject.Iterate();

        subject.Received().Pipe(Arg.Any<Iterate<char>>());
    }
}
