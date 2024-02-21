using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public static class OperandStreamGroupByExtensions {
    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, TKey> keySelector)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector));

    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector, keyComparer));

    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, Task<TKey>> keySelector)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector));

    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector, keyComparer));

    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task<TKey>> keySelector)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector));

    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector, keyComparer));
}
