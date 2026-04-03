using System.Net;

namespace BaseLibrary.Tools;

public class LaravelJsonResponse
{
    public object? Data { get; set; }
    public string Message { get; set; } = null!;
    public HttpStatusCode Status { get; set; }
    //public int Status { get; set; }
}