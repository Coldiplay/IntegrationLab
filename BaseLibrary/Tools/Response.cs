using System.Net;

namespace BaseLibrary.Tools;

public class Response
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public string? DataTypeName { get; set; } = null;
    public object? Data { get; set; } = null;
}