namespace VDT.Core.Operators;

/// <summary>
/// Options for an <see cref="IOperandStream{TValue}"/>
/// </summary>
public class OperandStreamOptions {
    /// <summary>
    /// Publish all published values to a new subscriber when it is added
    /// </summary>
    public bool ReplayWhenSubscribing { get; init; }
}
