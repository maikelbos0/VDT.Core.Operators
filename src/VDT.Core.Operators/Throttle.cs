﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace VDT.Core.Operators;

public class Throttle<TValue> : IOperator<TValue, TValue> {
    // TODO extract to central place
    internal Func<int, Task> Delay { get; set; } = Task.Delay;
    internal Func<DateTime> UtcNow { get; set; } = () => DateTime.UtcNow;

    private readonly Func<TValue, Task<int>> delayFunc;
    private int operationId = 0;
    private DateTime nextExpectedExecutionTime = DateTime.MinValue;

    public Throttle(int delayInMilliseconds)
        : this(value => Task.FromResult(delayInMilliseconds)) { }

    public Throttle(Func<TValue, int> delayFunc)
        : this(value => Task.FromResult(delayFunc(value))) { }

    public Throttle(Func<TValue, Task<int>> delayFunc) {
        this.delayFunc = delayFunc;
    }

    public async Task<OperationResult<TValue>> Execute(TValue value) {
        var now = UtcNow();
        var requiredDelayInMilliseconds = (nextExpectedExecutionTime - now).TotalMilliseconds;

        if (requiredDelayInMilliseconds > 0) {
            // Since Interlocked.Increment wraps, this will throttle properly until 2^32 operations occur in the delay
            var expectedOperationId = Interlocked.Increment(ref operationId);

            await Delay((int)requiredDelayInMilliseconds);

            if (operationId != expectedOperationId) {
                return OperationResult<TValue>.Dismissed();
            }
        }

        nextExpectedExecutionTime = UtcNow().AddMilliseconds(await delayFunc(value));

        return OperationResult<TValue>.Accepted(value);
    }
}
