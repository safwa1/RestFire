using System;

namespace RestFire.Attribute;

/// <summary>
/// Specifies that this field is a primary key in the database
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class KeyAttribute : System.Attribute
{

    public KeyAttribute()
    {
        AutoGenerate = true;
    }
        
    public KeyAttribute(bool autoGenerate)
    {
        AutoGenerate = autoGenerate;
    }

    public bool AutoGenerate { get; }
}