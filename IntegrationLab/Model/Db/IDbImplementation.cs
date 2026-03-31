using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationLab.Model.Db;

[Obsolete("Хз надо ли")]
public interface IDbAsyncImplementation<TModel> 
    where TModel : class, new()
{
    Task<List<TModel>> FetchAllAsync();
    Task<TModel> FetchByIdAsync(string id);
    Task<bool> UpdateAsync(TModel model);
    Task<TModel> CreateAsync(TModel model);
    Task<bool> KillAsync(TModel model);
}