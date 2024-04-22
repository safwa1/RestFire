using System;

namespace RestFire.Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class CollectionAttribute : System.Attribute
{
    /// <summary>
    /// Creates a collection mapping to a specific name for RestFire commands
    /// </summary>
    /// <param name="collectionName">The name of this collection in the database.</param>
    public CollectionAttribute(string collectionName)
    {
        Name = collectionName;
    }

    /// <summary>
    /// The name of the collectionName in the database
    /// </summary>
    public string Name { get; set; }
}