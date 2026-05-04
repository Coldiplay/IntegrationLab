using System.Collections;
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
            var response = await client.PostAsJsonAsync(url, parameter);
            var responseString = await response.Content.ReadAsStringAsync();
            return LaravelParser.ParseResponse<TResult>(responseString);
        }
    }
    extension(Microsoft.AspNetCore.SignalR.Hub hub)
    {
        public Response ToResponseWithData<T>(T? model = default, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
            where  T : notnull
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
    extension(IDictionary dictionary)
    {
        public T? TryGetValue<T>(string key, T? defaultValue = default)
        {
            return dictionary.Contains(key) ? (T?)dictionary[key] : defaultValue;
        }

        public bool TryGetValue<T>(string key, out T value)
            where T : notnull
        {
            try
            {
                if (dictionary.Contains(key))
                {
                    value = (T)(dictionary[key] ?? throw new KeyNotFoundException($"Key {key} not found"));
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            value = default;
            return false;
        }
    }
}