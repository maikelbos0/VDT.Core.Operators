using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamMergeExtensionsTests {
    [Fact]
    public void Merge() {
        var subject = Substitute.For<IOperandStream<string>>();
        var otherStreams = new[] {
            Substitute.For<IOperandStream<string>>(),
            Substitute.For<IOperandStream<string>>()
        };

        var result = subject.Merge(otherStreams);

        subject.Received().Subscribe((Func<string, CancellationToken, Task>)result.Publish);
        foreach (var otherStream in otherStreams) {
            otherStream.Received().Subscribe((Func<string, CancellationToken, Task>)result.Publish);
        }
    }
}
