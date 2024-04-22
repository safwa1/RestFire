using System;

namespace RestFire.Types;

public sealed class InfoStore
{
    private static readonly Lazy<InfoStore> Instance = new(() => new InfoStore());

    public static InfoStore Shared => Instance.Value;

    public string ApiKey { get; set; }
    public string DatabaseName { get; set; }
    public string AccessToken { get; set; }
}