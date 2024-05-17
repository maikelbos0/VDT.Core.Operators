using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace VDT.Core.Operators.Tests;

public class SubscriptionTests {
    [Fact]
    public async Task Unsubscribes() {
        var operandStream = new OperandStream<string>();
        var subscriber = Substitute.For<Action<string>>();
        var subject = operandStream.Subscribe(subscriber);

        subject.Unsubscribe();

        await operandStream.Publish("Foo");

        subscriber.DidNotReceive().Invoke(Arg.Any<string>());
        Assert.Null(subject.OperandStream);
    }
}
