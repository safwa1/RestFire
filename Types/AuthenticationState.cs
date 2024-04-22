namespace RestFire.Types;

public readonly record struct AuthenticationState(
    bool Success,
    string AccessToken = null
);