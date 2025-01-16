using System;
using System.Collections.Generic;

namespace VDT.Core.Operators;

/// <summary>
/// Options for an <see cref="IOperandStream{TValue}"/>
/// </summary>
public class OperandStreamOptions<TValue> {
    /// <summary>
    /// Gets or sets the setting to publish all previously published values to a new subscriber when it is added
    /// </summary>
    public bool ReplayWhenSubscribing { get; init; }

    /// <summary>
    /// Gets or sets the optional method that generates values when an <see cref="IOperandStream{TValue}"/> is first subscribed to
    /// </summary>
    public Func<IAsyncEnumerable<TValue>>? ValueGenerator { get; init; }
}
