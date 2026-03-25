using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;

namespace BaseLibrary.Tools;

public static class GlobalOptions
{
    public const string API_URI = "https://localhost:7106/";
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.Preserve
    };

    public static Faker Faker { get; } = new("ru");
    
    // static GlobalOptions()
    // {
    //     JsonSerializerOptions = new JsonSerializerOptions
    //     {
    //         ReferenceHandler = ReferenceHandler.Preserve,
    //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //     };
    // }
}