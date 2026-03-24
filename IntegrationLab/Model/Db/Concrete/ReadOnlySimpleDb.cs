using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationLab.Model.Db.Concrete;

public class ReadOnlySimpleDb(HttpClient httpClient) : SimpleApiDbHandler(httpClient)
{
    public async override Task<bool> KillAsync<T>(T model, string? tableName = null) => false;
    public async override Task<T?> CreateAsync<T>(T model, string? tableName = null) where T : class => null;
    public async override Task<bool> UpdateAsync<T>(T model, string? tableName = null) => false;
}