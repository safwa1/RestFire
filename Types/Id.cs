using System;

namespace RestFire.Types;

public readonly record struct Id : IValueObject<string>
{
    public string Value { get; }
    
    private Id(string value) => Value = value;
    
    //Operators
    public static implicit operator Id(string value) => new(value);
    public static implicit operator string(Id obj) => obj.Value;
    
    //Static
    // ReSharper disable once UnassignedReadonlyField
    public static readonly Id Empty;
    public static Guid ToGuid(string val) => Guid.Parse(val);
    public static Id Next() => new(Guid.NewGuid().ToString("N").ToUpper());

}