using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationLab.Model.Db.Concrete;

[Obsolete("Использовать HubHandler")]
public class ReadOnlySimpleDb(HttpClient httpClient) : SimpleApiDbHandler(httpClient)
{
    public override async Task<bool> KillAsync<T>(T model, string? tableName = null)
    {
        return false;
    }

    public override async Task<T?> CreateAsync<T>(T model, string? tableName = null) where T : class
    {
        return null;
    }

    public override async Task<bool> UpdateAsync<T>(T model, string? tableName = null)
    {
        return false;
    }
}