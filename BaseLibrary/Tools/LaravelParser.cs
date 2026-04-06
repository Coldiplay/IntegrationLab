using System.Net;
using System.Text.Json;

namespace BaseLibrary.Tools;

public static class LaravelParser
{
    private static readonly JsonSerializerOptions Options = new() {
        PropertyNameCaseInsensitive = true
    };
    
    public static T? ParseResponse<T>(string json)
    {
        var test = JsonSerializer.Deserialize<LaravelJsonResponse>(json, Options);

        if (test!.Status == (HttpStatusCode)201 && test?.Data is not null)
        {
            var typeName = test.Type?.Remove(0, test.Type.LastIndexOf('\\') + 1).Replace("Resource", "");
            var type = FindType(typeName!);
            if (type is null) throw new Exception();
            if (type == typeof(T))
            {
                var element = JsonElement.Parse(test.Data.ToString()!);
                var model = element.Deserialize<T>(Options);
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

    private static readonly Type[] Types = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "BaseLibrary")
        .GetTypes().Where(t => t.FullName!.Contains("BaseLibrary.Model")).ToArray();
    private static readonly Dictionary<string, Type> KnownTypes = [];
    private static Type? FindType(string typeName)
    {
        if (KnownTypes.TryGetValue(typeName, out var findType)) return findType;

        Type? typee = null;
        foreach (var type in Types)
        {
            if (!type.Name.Contains(typeName)) continue;
            
            if (typee is not null)
            {
                throw new Exception();
            }
            typee = type;
        }

        if (typee is not null) KnownTypes.Add(typeName, typee);
        
        return typee;
    }
}