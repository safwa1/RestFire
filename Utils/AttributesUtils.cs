using System;
using System.Reflection;
using RestFire.Attribute;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RestFire.Utils;

public static class AttributesUtils
{
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeCollectionName = new();
    public delegate string CollectionNameMapperDelegate(Type type);

#pragma warning disable CA2211 // Non-constant fields should not be visible - I agree with you, but we can't do that until we break the API
    public static CollectionNameMapperDelegate CollectionNameMapper;
#pragma warning restore CA2211 // Non-constant fields should not be visible
    
    public static string GetCollectionName(Type type)
    {
        if (TypeCollectionName.TryGetValue(type.TypeHandle, out string name)) return name;

        if (CollectionNameMapper != null)
        {
            name = CollectionNameMapper(type);
        }
        else
        {
            var CollectionAttrName =
                type.GetCustomAttribute<CollectionAttribute>(false)?.Name
                ?? (type.GetCustomAttributes(false).FirstOrDefault(attr => attr.GetType().Name == "CollectionAttribute") as dynamic)?.Name;

            if (CollectionAttrName != null)
            {
                name = CollectionAttrName;
            }
            else
            {
                name = type.Name + "s";
                if (type.IsInterface && name.StartsWith("I"))
                    name = name.Substring(1);
            }
        }

        TypeCollectionName[type.TypeHandle] = name;
        return name;
    }
    
    private static List<PropertyInfo> KeyPropertiesCache(Type type)
    {
        if (KeyProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
        {
            return pi.ToList();
        }

        var allProperties = TypePropertiesCache(type);
        var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

        if (keyProperties.Count == 0)
        {
            var idProp = allProperties.Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
            if (idProp != null && !idProp.GetCustomAttributes(true).Any(a => a is ExplicitKeyAttribute))
            {
                keyProperties.Add(idProp);
            }
        }

        KeyProperties[type.TypeHandle] = keyProperties;
        return keyProperties;
    }

    private static List<PropertyInfo> TypePropertiesCache(Type type)
    {
        if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
        {
            return pis.ToList();
        }

        var properties = type.GetProperties().Where(IsWriteable).ToArray();
        TypeProperties[type.TypeHandle] = properties;
        return properties.ToList();
    }
    
    private static bool IsWriteable(PropertyInfo pi)
    {
        var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false).AsList();
        if (attributes.Count != 1) return true;

        var writeAttribute = (WriteAttribute)attributes[0];
        return writeAttribute.Write;
    }
    
    public static PropertyInfo GetSingleKey<T>(string method)
    {
        var type = typeof(T);
        var keys = KeyPropertiesCache(type);

        return keys.Count > 0 ? keys[0] : throw new DataException($"{method}<T> only supports an entity with a [Key] property");
    }
    
    public static PropertyInfo GetSingleKey(Type type)
    {
        var keys = KeyPropertiesCache(type);

        return keys.Count > 0 ? keys[0] : throw new DataException($"only supports an entity with a [Key] property");
    }
    
    private static KeyAttribute GetKeyAttribute(PropertyInfo property)
    {
        KeyAttribute keyAttribute = (KeyAttribute)property.GetCustomAttribute(typeof(KeyAttribute));
        return keyAttribute;
    }

    private static bool FilterKeyAttribute(PropertyInfo propertyInfo)
    {
        var keyAttribute = GetKeyAttribute(propertyInfo);
        return keyAttribute.AutoGenerate;
    }
}