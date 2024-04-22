#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestFire.Utils;

public static class Extensions
{
    public static List<T> ToList<T>(this string value)
    {
        var data = JsonConvert.DeserializeObject<JObject>(value);
        var recordsList = new List<T>();
        if (data == null) return recordsList;
        foreach (var pair in data)
        {
            var key = pair.Key;
            var pairValue = pair.Value;

            if (pairValue == null) continue;
            var item = pairValue.ToObject<T>();
            item?.GetType().GetProperty("Id")?.SetValue(item, key);
            if (item != null) recordsList.Add(item);
        }

        return recordsList;
    }

    public static TValue To<TValue, TKey>(this string value) where TValue : class
    {
        var data = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(value);
        if (data == null || data.Count == 0) return null!;

        // Get the first (and presumably only) entry in the dictionary
        var entry = data.First();

        if (entry.Value == null) return null!;

        // Assuming the dictionary key is a property of the value type
        PropertyInfo keyProperty = typeof(TValue).GetProperty("Id") ?? throw new InvalidOperationException();
        keyProperty.SetValue(entry.Value, entry.Key);

        return entry.Value;
    }

    public static T ToObject<T>(this string value)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<string, T>>(value);
        if (data == null || data.Count == 0) return default!;

        // Get the first (and presumably only) entry in the dictionary
        var entry = data.First();

        if (entry.Value == null) return default!;

        // If T has a property named 'Id', set its value to the key
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(entry.Value, Convert.ChangeType(entry.Key, idProperty.PropertyType));
        }

        return entry.Value;
    }

    public static T? MapTo<T>(this string json)
        => JsonConvert.DeserializeObject<T>(json);

    public static string ToJson<T>(this T type) => JsonConvert.SerializeObject(type);

    public static string RemoveInvalidChars(this string input)
    {
        var invalidChars = new HashSet<char>(new[]
        {
            '\\', '/', ':', '*', '?', '"', '<', '>', '.', '_', '-', '=', '+'
        });

        var sanitizedString = new StringBuilder(input.Length);

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsWhiteSpace(c)) continue;
            if (!invalidChars.Contains(c))
            {
                sanitizedString.Append(
                    char.ToUpperInvariant(c)
                );
            }
        }

        return sanitizedString.ToString();
    }
    
    public static List<T> AsList<T>(this IEnumerable<T>? source)
    {
        return source == null ? (List<T>) null : (source is List<T> objList ? objList : source.ToList<T>());
    }
}