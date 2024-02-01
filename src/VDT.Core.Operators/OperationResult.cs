using System;

namespace VDT.Core.Operators;

public class OperationResult<TNewValue> {
    public static OperationResult<TNewValue> Accepted(TNewValue value) => new(true, value);
    public static OperationResult<TNewValue> Dismissed() => new(false, default);

    private readonly TNewValue? value;

    public bool IsAccepted { get; }
    public TNewValue Value => value ?? throw new InvalidOperationException("Result was not accepted; value is not available");

    private OperationResult(bool isAccepted, TNewValue? value) {
        IsAccepted = isAccepted;
        this.value = value;
    }
}
