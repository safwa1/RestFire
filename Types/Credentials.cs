namespace RestFire.Types;

public sealed record Credentials(
    string Email,
    string Password,
    string Token
);