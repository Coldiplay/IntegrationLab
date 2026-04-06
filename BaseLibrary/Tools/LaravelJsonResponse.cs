using System.Net;

namespace BaseLibrary.Tools;

public class LaravelJsonResponse
{
    public object? Data { get; init; }
    public string? Type { get; init; }
    public object? Relationships { get; init; }
    public string Message { get; init; } = null!;
    public HttpStatusCode Status { get; init; }
    //public int Status { get; set; }
}