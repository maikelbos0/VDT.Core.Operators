namespace VDT.Core.Operators;

/// <summary>
/// Options for an <see cref="IOperandStream{TValue}"/>
/// </summary>
public class OperandStreamOptions {
    /// <summary>
    /// Gets or sets the setting to publish all previously published values to a new subscriber when it is added
    /// </summary>
    public bool ReplayWhenSubscribing { get; init; }
}
