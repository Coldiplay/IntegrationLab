using System.Net;
using BaseLibrary.Tools;

namespace MobileSignalR.Hub;

public static class Extensions
{
    extension(HttpClient client)
    {
        public async Task<TResult?> GetLaravel<TResult>(string url)
        {
            return LaravelParser.ParseResponse<TResult>(
                await (await client.GetAsync(url))
                    .Content.ReadAsStringAsync());
        }

        public async Task<TResult?> PostLaravel<TResult>(string url, object parameter)
        {
            return LaravelParser.ParseResponse<TResult>(
                await (await client.PostAsJsonAsync(url, parameter))
                    .Content.ReadAsStringAsync());
        }
    }

    extension(Microsoft.AspNetCore.SignalR.Hub hub)
    {
        public Response ToResponseWithData<T>(T? model = default, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (model is null)
            {
                return new Response()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = message ?? "Not found"
                };
            }

            var typeName = typeof(T).Name;
            return new Response()
            {
                StatusCode = statusCode,
                Data = model,
                DataTypeName = typeName,
                Message = message ?? $"Successful retrieved {typeName}"
            };
        }

        public Response BadResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new Response()
            {
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}