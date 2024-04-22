using System;
using RestFire.Types;
using RestFire.Utils;

namespace RestFire.Http.Controllers;

public abstract class BaseController<T>
{
    protected T Deleted { get; set; }

    public event EventHandler<DataChangeEventArg<T>> DataChanged;

    protected void Emit(DbActionType dbActionType, T entry = default) =>
        OnDataChanged(new DataChangeEventArg<T>(dbActionType, entry));

    private void OnDataChanged(DataChangeEventArg<T> e) => DataChanged?.Invoke(this, e);
    
    protected static string GetEntityId(T entity)
    {
        // Implement logic to get the ID from the entity
        // For simplicity, assuming there's a property named "Id" in the entity
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var idValue = idProperty.GetValue(entity)?.ToString();
            if (!string.IsNullOrEmpty(idValue))
            {
                return idValue;
            }
        }

        // Implement logic to handle cases where the ID cannot be retrieved
        // You may throw an exception or handle it based on your requirements
        throw new InvalidOperationException("Unable to determine entity Id.");
    }

    protected static string GetKeyValue(T entity)
    {
        var propertyInfo = AttributesUtils.GetSingleKey<T>(nameof(GetKeyValue));
        if (propertyInfo != null)
        {
            var idValue = propertyInfo.GetValue(entity)?.ToString();
            if (!string.IsNullOrEmpty(idValue))
            {
                return idValue;
            }
        }
        
        throw new InvalidOperationException("Unable to determine entity Id.");
    }
}