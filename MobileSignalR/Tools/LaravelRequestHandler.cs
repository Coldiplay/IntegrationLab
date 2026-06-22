using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using BaseLibrary.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace MobileSignalR.Tools;

public class LaravelRequestHandler(
    ILogger<LaravelRequestHandler> logger,
    HttpClient apiClient,
    JwtTokenHandler tokenHandler)
{
    private readonly JsonSerializerSettings _options = new() {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
    };
    
    private readonly Type[] Types = AppDomain.CurrentDomain.GetAssemblies()
        .First(a => a.GetName().Name == "BaseLibrary")
        .GetTypes().Where(t => t.FullName!.Contains("BaseLibrary.Model")).ToArray();

    private readonly ConcurrentDictionary<string, Type> KnownTypes = [];
    private readonly Type _iEnumerableType =  typeof(IEnumerable);
    private readonly Type _enumerableType =  typeof(Enumerable);
    
    public async Task<TResult?> Get<TResult>(string url, string? token = null)
    {
        SwitchTokenToCurrentUser(token);
        
        var response = await apiClient.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();
        return ParseResponse<TResult>(responseContent);
    }

    public async Task<TResult?> Post<TResult>(string url, object parameter, string? token = null)
    {
        SwitchTokenToCurrentUser(token);

        var jsonPayload = JsonConvert.SerializeObject(parameter, _options);
        using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        
        var response = await apiClient.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();
        return ParseResponse<TResult>(responseString);
    }
    
    private T? ParseResponse<T>(string json)
    {
        var laravelResponse = JsonConvert.DeserializeObject<LaravelJsonResponse>(json, _options)!;

        if ((laravelResponse.Status != HttpStatusCode.OK 
             && laravelResponse.Status != HttpStatusCode.Created) 
            || laravelResponse.Data is null) 
            return default;
        
        try
        {
            var strData = laravelResponse.Data.ToString()!;

            if (laravelResponse.ContainerType!.Equals("object"))
            {
                var model = JsonConvert.DeserializeObject<T>(strData, _options);
                return model;
            }
                
            try
            {
                var rootObjType = FindType(laravelResponse.ClassType!);
                var returnList = new ArrayList();
                var elements = JArray.Parse(strData);
                
                foreach (var element in elements)
                {
                    var returnElement = JsonConvert.DeserializeObject(element.ToString(), rootObjType, _options);
                            
                    FillRelationships(returnElement!, element, rootObjType!);
                            
                    returnList.Add(returnElement);
                }
                
                
                var returnType = typeof(T);

                if (!_iEnumerableType.IsAssignableFrom(returnType))
                    return returnList.Count > 0
                        ? (T)returnList[0]!
                        : default;
                        
                var u = returnType.GetGenericArguments()[0];
                var cast = _enumerableType
                    .GetMethod(nameof(Enumerable.Cast))!
                    .MakeGenericMethod(u);
                var result = (IEnumerable)cast.Invoke(null, [returnList]);
                //var test = result.Cast<object>().ToList();
                //var test2 = returnList.Cast<object>().ToList();
                //return (T)returnList.Cast<object>(); //Желательно так сделать, но cast озвращает чутьчуть не IEnumerable...
                return (T?)result;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return default;
            }
        }
        catch (Exception e)
        {
            var typeName = laravelResponse.ClassType?.Remove(0, laravelResponse.ClassType.LastIndexOf('\\') + 1).Replace("Resource", "").Replace("Collection", "");
            var type = FindType(typeName!);
            if (type == typeof(T))
            {
                var element = JToken.Parse(laravelResponse.Data.ToString()!);
                var model = element.ToObject<T>()!;
                FillRelationships(model, element, type);
                //FillWithRelations((model, type), JsonElement.Parse(test.Relationships?.ToString()!));
                return model;
            }

            logger.LogError(e, "{error}", e.Message);
        }

        return default;
    }

    private void FillRelationships(object model, JToken modelJObj, Type modelType)
    {
        var elemObj = modelJObj.ToObject<JObject>();
        
        if (!elemObj!.ContainsKey("relationships")) return;
        
        var relations = elemObj["relationships"]!.ToObject<JObject>()!;
        foreach (var relationProp in relations.Properties())
        {
            var prop = modelType?.GetProperties()
                .FirstOrDefault(p =>
                    p.Name.Equals(relationProp.Name.ToPascalCase(),
                        StringComparison.OrdinalIgnoreCase));

            if (prop is null) continue;

            var propValue = JsonConvert.DeserializeObject(relationProp.Value.ToString(), prop.PropertyType, _options);
            prop.SetValue(model, propValue);
        }
    }

    private void SwitchTokenToCurrentUser(string? token)
    {
        if (string.IsNullOrEmpty(token)) return;
        var laraToken = tokenHandler.GetAuthToken(token);
        apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", laraToken);
    }
    private Type? FindType(string typeName)
    {
        if (KnownTypes.TryGetValue(typeName, out var foundType)) return foundType;

        Type? returnType = null;
        foreach (var type in Types)
        {
            var cleanClassName = type.FullName!.Remove(0, type.FullName.LastIndexOf('.') + 1).Replace("+<>c", "");
            if (!cleanClassName.Equals(typeName)) continue;

            returnType = type;
            break;
        }

        if (returnType is not null) 
            _ = KnownTypes.GetOrAdd(typeName, returnType);

        return returnType;
    }
}