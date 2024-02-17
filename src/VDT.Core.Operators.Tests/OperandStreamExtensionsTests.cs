using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class OperandStreamExtensionsTests {
    [Fact]
    public async Task PublishesToVoidOperandStream() {
        var subject = Substitute.For<IOperandStream<Void>>();

        await subject.Publish();

        await subject.Received().Publish(Arg.Any<Void>());
    }

    [Fact]
    public async Task CancellablePublishesToVoidOperandStream() {
        var cancellationTokenSource = new CancellationTokenSource();
        var subject = Substitute.For<IOperandStream<Void>>();

        await subject.Publish(cancellationTokenSource.Token);

        await subject.Received().Publish(Arg.Any<Void>(), cancellationTokenSource.Token);
    }
}
