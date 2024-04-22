using System;

namespace RestFire.Http;


internal sealed class FirebaseDatabase
{
    private Axios _axios;
    private string DatabaseName { get; set; }

    private static readonly Lazy<FirebaseDatabase> LazyInstance = new(() => new FirebaseDatabase());

    public static FirebaseDatabase Instance => LazyInstance.Value;

    private FirebaseDatabase(){}

    public FirebaseDatabase SetDatabaseName(string name)
    {
        DatabaseName = name;
        return this;
    }

    private string CreateUrl() => $"https://{DatabaseName}.firebaseio.com/";

    public Axios GetDatabase()
    {
        _axios ??= new Axios(CreateUrl());
        return _axios;
    }
}