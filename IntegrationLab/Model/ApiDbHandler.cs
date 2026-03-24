using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IntegrationLab.Model;

public class ApiDbHandler(HttpClient httpClient)
{
    //public virtual async Task<List<TModel>> FetchAllAsync() => await FetchAllFromJsonAsync<List<TModel>>();
    //public virtual async Task<Shipping?> FetchShippingById(Guid id) => await FetchByIdFromJsonAsync<Shipping>(id.ToString());
    //public virtual async Task<bool> Update(Shipping shipping) => await UpdateAsync(shipping);

    public virtual async Task<T?> FetchByIdFromJsonAsync<T>(string id, string? tableName = null)
    where T : class, new()
    {
        tableName ??= typeof(T).Name + 's';
        try
        {
            var response = await httpClient.GetFromJsonAsync<T>($"api/{tableName}/{id}");
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            return null;
        }
    }
    
    public virtual async Task<T> FetchAllFromJsonAsync<T>(string? tableName = null)
    where T : IList, new()
    {
        var type = typeof(T);

        try
        {
            tableName ??= type.GetGenericArguments()[0].Name + 's';
            return await httpClient.GetFromJsonAsync<T>($"api/{tableName}") ?? new T();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            return new T();
        }
    }

    public virtual async Task<bool> UpdateAsync<T>(T model, string? tableName = null)
    where T : class, new()
    {
        tableName ??= typeof(T).Name + 's';
        try
        {
            //TODO: Проверить работоспособность
            var response = await httpClient.PutAsJsonAsync($"api/{tableName}", model);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            return false;
        }
    }

    public virtual async Task<T?> CreateAsync<T>(T model, string? tableName = null)
    where T : class, new()
    {
        tableName ??= typeof(T).Name + 's';
        try
        {
            //TODO: Проверить работоспособность
            var response = await httpClient.PostAsJsonAsync($"api/{tableName}", model);
            return (await response.Content.ReadFromJsonAsync<T>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            return null;
        }
    }

    public virtual async Task<bool> KillAsync<T>(T model, string? tableName = null)
    {
        var type = typeof(T);
        tableName ??= type.Name + 's';
        
        try
        {
            //TODO: Проверить работоспособность
            var id = type.GetProperty("Id")?.GetValue(model);
            var response = await httpClient.DeleteAsync($"api/{tableName}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            return false;
        }
    }
    
    /*
    private async Task<T?> FetchAllFromJsonAsync<T>(string? tableName = null)
    {
        var type = typeof(T);
        
        tableName ??= type.BaseType == typeof(List<>)
            ? type.GetGenericArguments()[0].Name + 's'
            : tableName = typeof(T).Name + 's';
        
        
        return await _httpClient.GetFromJsonAsync<T>($"api/{tableName}");
    }
    */
}