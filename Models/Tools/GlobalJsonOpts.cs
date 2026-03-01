using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models.Tools;

public static class GlobalJsonOpts
{
    public static JsonSerializerOptions JsonSerializerOptions { get; }

    static GlobalJsonOpts()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}