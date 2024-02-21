using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

/// <summary>
/// Extension methods to more fluently apply operators to <see cref="IOperandStream{TValue}"/>
/// </summary>
public static class OperandStreamGroupByExtensions {
    /// <summary>
    /// Saves up values in a list, only publishing the previously saved values when the key changes
    /// </summary>
    /// <typeparam name="TValue">Type of value to group</typeparam>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that lists of <typeparamref name="TValue"/> will be published to</returns>
    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, TKey> keySelector)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector));

    /// <summary>
    /// Saves up values in a list, only publishing the previously saved values when the key changes
    /// </summary>
    /// <typeparam name="TValue">Type of value to group</typeparam>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <param name="keyComparer"><see cref="IEqualityComparer{T}"/> to use when comparing the key</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that lists of <typeparamref name="TValue"/> will be published to</returns>
    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector, keyComparer));

    /// <summary>
    /// Saves up values in a list, only publishing the previously saved values when the key changes
    /// </summary>
    /// <typeparam name="TValue">Type of value to group</typeparam>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that lists of <typeparamref name="TValue"/> will be published to</returns>
    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, Task<TKey>> keySelector)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector));

    /// <summary>
    /// Saves up values in a list, only publishing the previously saved values when the key changes
    /// </summary>
    /// <typeparam name="TValue">Type of value to group</typeparam>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <param name="keyComparer"><see cref="IEqualityComparer{T}"/> to use when comparing the key</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that lists of <typeparamref name="TValue"/> will be published to</returns>
    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector, keyComparer));

    /// <summary>
    /// Saves up values in a list, only publishing the previously saved values when the key changes
    /// </summary>
    /// <typeparam name="TValue">Type of value to group</typeparam>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that lists of <typeparamref name="TValue"/> will be published to</returns>
    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task<TKey>> keySelector)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector));

    /// <summary>
    /// Saves up values in a list, only publishing the previously saved values when the key changes
    /// </summary>
    /// <typeparam name="TValue">Type of value to group</typeparam>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <param name="operandStream"><see cref="IOperandStream{TValue}"/> that will be subscribed to</param>
    /// <param name="keySelector">Method that returns the key for a given value</param>
    /// <param name="keyComparer"><see cref="IEqualityComparer{T}"/> to use when comparing the key</param>
    /// <returns><see cref="IOperandStream{TValue}"/> that lists of <typeparamref name="TValue"/> will be published to</returns>
    public static IOperandStream<List<TValue>> GroupBy<TValue, TKey>(this IOperandStream<TValue> operandStream, Func<TValue, CancellationToken, Task<TKey>> keySelector, IEqualityComparer<TKey> keyComparer)
        => operandStream.Pipe(new GroupBy<TValue, TKey>(keySelector, keyComparer));
}
