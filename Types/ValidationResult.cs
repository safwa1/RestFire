namespace RestFire.Types;

public sealed record ValidationResult(
    bool IsValid,
    string Message
);