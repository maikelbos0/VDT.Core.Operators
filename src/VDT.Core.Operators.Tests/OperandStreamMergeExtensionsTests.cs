using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamMergeExtensionsTests {
    [Fact]
    public void Merge_Array() {
        var subject = Substitute.For<IOperandStream<string>>();
        var additionalStreams = new[] {
            Substitute.For<IOperandStream<string>>(),
            Substitute.For<IOperandStream<string>>()
        };

        _ = subject.Merge(additionalStreams);

        subject.Received().Pipe(Arg.Any<Merge<string>>(), additionalStreams);
    }

    [Fact]
    public void Merge_Enumerable() {
        var subject = Substitute.For<IOperandStream<string>>();
        var additionalStreams = new List<IOperandStream<string>>() {
            Substitute.For<IOperandStream<string>>(),
            Substitute.For<IOperandStream<string>>()
        };

        _ = subject.Merge(additionalStreams);

        subject.Received().Pipe(Arg.Any<Merge<string>>(), additionalStreams);
    }
}
