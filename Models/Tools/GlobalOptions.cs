using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models.Tools;

public static class GlobalOptions
{
    public const string API_URI = "https://localhost:7106/";
    public static JsonSerializerOptions JsonSerializerOptions { get; }

    static GlobalOptions()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}