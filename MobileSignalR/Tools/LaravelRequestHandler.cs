using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
    private readonly JsonSerializerSettings _options = new()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
    };
    
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

    private void SwitchTokenToCurrentUser(string? token)
    {
        if (string.IsNullOrEmpty(token)) return;
        var laraToken = tokenHandler.GetAuthToken(token);
        apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", laraToken);
    }
    

    private T? ParseResponse<T>(string json)
    {
        var laravelResponse = JsonConvert.DeserializeObject<LaravelJsonResponse>(json, _options);
        
        if (((int)laravelResponse!.Status == 200 || (int)laravelResponse.Status == 201) && laravelResponse?.Data is not null)
            try
            {
                var strData = (laravelResponse.Data.ToString())!;
                if (laravelResponse.ContainerType!.Equals("array"))
                {
                    try
                    {
                        var rootObjType = FindType(laravelResponse.ClassType!);
                        var returnList = new ArrayList();
                        var elements = JArray.Parse(strData);
                        foreach (var element in elements)
                        {
                            var returnElement = JsonConvert.DeserializeObject(element.ToString(), rootObjType, _options);
                            var elemObj = element.ToObject<JObject>();
                            if (elemObj!.ContainsKey("relationships"))
                            {
                                var relations = elemObj["relationships"]!.ToObject<JObject>()!;
                                foreach (var relationProp in relations.Properties())
                                {
                                    var prop = rootObjType?.GetProperties()
                                        .FirstOrDefault(p =>
                                            p.Name.Equals(relationProp.Name.ToPascalCase(),
                                                StringComparison.OrdinalIgnoreCase));

                                    if (prop is null) continue;

                                    var propValue =
                                        JsonConvert.DeserializeObject(relationProp.Value.ToString(), prop.PropertyType, _options);
                                    //var propValue = relationProp.Value.ToObject(prop.PropertyType);
                                    prop.SetValue(returnElement, propValue);
                                }
                            }
                            returnList.Add(returnElement);
                        }
                        
                        var tType = typeof(T);

                        if (!_iEnumerableType.IsAssignableFrom(tType))
                            return returnList.Count > 0
                                ? (T)returnList[0]!
                                : default;
                        
                        var u = tType.GetGenericArguments()[0];
                        var cast = _enumerableType
                            .GetMethod(nameof(Enumerable.Cast))!
                            .MakeGenericMethod(u);
                        var result = (IEnumerable)cast.Invoke(null, [returnList]);
                        var test = result.Cast<object>().ToList();
                        return (T)result!;
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, e.Message);
                        return default;
                    }
                }
                
                var model = JsonConvert.DeserializeObject<T>(strData!, _options);
                return model;
            }
            catch (Exception e)
            {
                var typeName = laravelResponse.ClassType?.Remove(0, laravelResponse.ClassType.LastIndexOf('\\') + 1).Replace("Resource", "");
                var type = FindType(typeName!);
                if (type is null) throw new Exception();
                if (type == typeof(T))
                {
                    var element = JsonElement.Parse(laravelResponse.Data.ToString()!);
                    var model = element.Deserialize<T>();
                    //FillWithRelations((model, type), JsonElement.Parse(test.Relationships?.ToString()!));
                    return model;
                }
            }

        ;
        return default;
    }

    private readonly Type[] Types = AppDomain.CurrentDomain.GetAssemblies()
        .First(a => a.GetName().Name == "BaseLibrary")
        .GetTypes().Where(t => t.FullName!.Contains("BaseLibrary.Model")).ToArray();

    private readonly Dictionary<string, Type> KnownTypes = [];
    private readonly Type _iEnumerableType =  typeof(IEnumerable);
    private readonly Type _enumerableType =  typeof(Enumerable);

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

        if (returnType is not null) KnownTypes.Add(typeName, returnType);

        return returnType;
    }
}