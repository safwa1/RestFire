using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestFire.Types;
using RestFire.Utils;

namespace RestFire.Http.Controllers;

public class RestFireBaseController<T> : BaseController<T>
{
    private string _token = string.Empty;
    private readonly string _collectionName;
    private readonly FirebaseDatabase _firebaseDatabase = FirebaseDatabase.Instance;
    private static bool HasAccessToken => !string.IsNullOrEmpty(InfoStore.Shared.AccessToken);

    public RestFireBaseController(string collectionName = null)
    {
        _collectionName = collectionName ?? AttributesUtils.GetCollectionName(typeof(T));
        _firebaseDatabase.SetDatabaseName(InfoStore.Shared.DatabaseName);
        if (HasAccessToken)
        {
            UseToken(InfoStore.Shared.AccessToken);
        }
    }

    public RestFireBaseController<T> UseToken(string token)
    {
        _token = token;
        return this;
    }

    public async Task<List<T>> GetAllAsync()
    {
        var url = HasAccessToken
            ? $"{_collectionName}.json?auth={_token}"
            : $"{_collectionName}.json";
        var response = await _firebaseDatabase.GetDatabase().GetAsync(url);
        return response.IsSuccess
            ? response.ResponseText.ToList<T>()
            : [];
    }

    public async Task<T> GetByAsync(string key, string value)
    {
        var url = HasAccessToken
            ? $"{_collectionName}.json?auth={_token}&orderBy=\"{key}\"&equalTo=\"{value}\""
            : $"{_collectionName}.json&orderBy=\"{key}\"&equalTo=\"{value}\"";
        var response = await _firebaseDatabase.GetDatabase().GetAsync(
            url
        );

        return response.IsSuccess
            ? response.ResponseText.ToObject<T>()
            : default;
    }

    public async Task<T> GetAsync(string id)
    {
        var url = HasAccessToken
            ? $"{_collectionName}/{id}.json?auth={_token}"
            : $"{_collectionName}/{id}.json";
        var response = await _firebaseDatabase.GetDatabase().GetAsync(url);
        return response.IsSuccess
            ? response.ResponseText.MapTo<T>()
            : default;
    }
    
    public async Task<bool> DeleteAsync(T entry)
    {
        var delete = await DeleteByIdAsync(GetKeyValue(entry));
        if (delete) Emit(DbActionType.Delete, entry);
        return delete;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entry = await GetAsync(id);
        var delete = await DeleteByIdAsync(id);
        if (delete) Emit(DbActionType.Delete, entry);
        return delete;
    }

    public async Task<T> InsertAsync(T entity)
    {
        var url = HasAccessToken
            ? $"{_collectionName}/{GetKeyValue(entity)}.json?auth={_token}"
            : $"{_collectionName}/{GetKeyValue(entity)}.json";
        var response = await _firebaseDatabase.GetDatabase().PutAsync(
            url,
            entity.ToJson()
        );

        var createdEntity = response.IsSuccess ? response.ResponseText.MapTo<T>() : default;
        if (response.IsSuccess || createdEntity != null)
        {
            Emit(DbActionType.Create, createdEntity);
        }

        return createdEntity;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        var url = HasAccessToken
            ? $"{_collectionName}/{GetKeyValue(entity)}.json?auth={_token}"
            : $"{_collectionName}/{GetKeyValue(entity)}.json";
        var response = await _firebaseDatabase.GetDatabase().PutAsync(
            url,
            JsonConvert.SerializeObject(entity)
        );

        if (response.IsSuccess)
        {
            Emit(DbActionType.Update, entity);
        }

        return response.IsSuccess;
    }
    

    private async Task<bool> DeleteByIdAsync(string uid)
    {
        var url = HasAccessToken
            ? $"{_collectionName}/{uid}.json?auth={_token}"
            : $"{_collectionName}/{uid}.json";
        var response = await _firebaseDatabase.GetDatabase().DeleteAsync(
            url
        );

        return response.IsSuccess;
    }
}