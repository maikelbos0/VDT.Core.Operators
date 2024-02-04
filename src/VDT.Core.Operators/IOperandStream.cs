using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public interface IOperandStream<TValue> {
    Task Write(TValue value);
    Task Write(TValue value, CancellationToken cancellationToken);
    void Subscribe(Action<TValue> subscriber);
    void Subscribe(Func<TValue, Task> subscriber);
    void Subscribe(Func<TValue, CancellationToken, Task> subscriber);
    IOperandStream<TNewValue> Pipe<TNewValue>(IOperator<TValue, TNewValue> op);
}
