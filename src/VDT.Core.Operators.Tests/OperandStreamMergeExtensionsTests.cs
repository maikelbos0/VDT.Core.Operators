using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamMergeExtensionsTests {
    [Fact]
    public void Merge_Array() {
        var subject = Substitute.For<IOperandStream<string>>();
        var additionalStream1 = Substitute.For<IOperandStream<string>>();
        var additionalStream2 = Substitute.For<IOperandStream<string>>();

        _ = subject.Merge(additionalStream1, additionalStream2);

        subject.Received().Pipe(Arg.Any<Merge<string>>(), Arg.Is<IEnumerable<IOperandStream<string>>>(x => x.First() == additionalStream1 && x.Last() == additionalStream2));
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
