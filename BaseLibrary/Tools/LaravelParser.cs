using System.Collections;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BaseLibrary.Tools;

public static class LaravelParser
{
    private static readonly JsonSerializerSettings Options = new()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
    };

    public static T? ParseResponse<T>(string json)
    {
        var laravelResponse = JsonConvert.DeserializeObject<LaravelJsonResponse>(json, Options);
        
        if (((int)laravelResponse!.Status == 200 || (int)laravelResponse.Status == 201) && laravelResponse?.Data is not null)
            try
            {
                if (laravelResponse.ContainerType!.Equals("array"))
                {
                    var rootObjType = FindType(laravelResponse.ClassType!);
                    var returnList = new ArrayList();
                    var array = JArray.Parse(laravelResponse.Data.ToString()!);
                    foreach (var element in array)
                    {
                        var returnElement = element.ToObject<T>();
                        var elemObj = element.ToObject<JObject>();
                        if (elemObj!.ContainsKey("relationships"))
                        {
                            var relations = elemObj["relationships"]!.ToObject<JObject>()!;
                            foreach (var relationProp in relations.Properties())
                            {
                                var prop = rootObjType?.GetProperties()
                                    .FirstOrDefault(p =>
                                        p.Name.Contains(relationProp.Name,
                                            StringComparison.OrdinalIgnoreCase));
                                
                                prop?.SetValue(returnElement, relationProp.Value.ToObject(prop.PropertyType));
                            }
                        }
                        returnList.Add(returnElement);
                    }

                    if (typeof(IEnumerable).IsAssignableFrom(rootObjType))
                        return (T)(object)returnList;
                    return returnList.Count > 0
                        ? (T)returnList[0]!
                        : default;
                }
                
                var model = JsonConvert.DeserializeObject<T>(laravelResponse.Data.ToString()!, Options);
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

    private static void FillWithRelations<T>((T model, Type modelType) tuple, JsonElement data)
    {
        //TODO: Сделать наполнение relations
        var relations = data.Deserialize<object>();

        return;
    }

    private static readonly Type[] Types = AppDomain.CurrentDomain.GetAssemblies()
        .First(a => a.GetName().Name == "BaseLibrary")
        .GetTypes().Where(t => t.FullName!.Contains("BaseLibrary.Model")).ToArray();

    private static readonly Dictionary<string, Type> KnownTypes = [];

    private static Type? FindType(string typeName)
    {
        if (KnownTypes.TryGetValue(typeName, out var findType)) return findType;

        Type? typee = null;
        foreach (var type in Types)
        {
            if (!type.Name.Contains(typeName)) continue;

            if (typee is not null) throw new Exception();

            typee = type;
        }

        if (typee is not null) KnownTypes.Add(typeName, typee);

        return typee;
    }
}