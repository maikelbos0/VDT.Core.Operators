using System;
using System.Threading.Tasks;

namespace VDT.Core.Operators {
    public interface IOperandStream<TValue> {
        IOperandStream<TNewValue> Pipe<TNewValue>(IOperator<TValue, TNewValue> op);
        void Subscribe(Action<TValue> subscriber);
        void Subscribe(Func<TValue, Task> subscriber);
        Task Write(TValue value);
    }
}