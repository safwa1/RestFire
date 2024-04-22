using System;

namespace RestFire.Attribute;

/// <summary>
/// Specifies whether a field is writable in the database.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class WriteAttribute : System.Attribute
{
    /// <summary>
    /// Specifies whether a field is writable in the database.
    /// </summary>
    /// <param name="write">Whether a field is writable in the database.</param>
    public WriteAttribute(bool write)
    {
        Write = write;
    }

    /// <summary>
    /// Whether a field is writable in the database.
    /// </summary>
    public bool Write { get; }
}