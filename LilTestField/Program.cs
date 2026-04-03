using System.Net;
using System.Text.Json;
using BaseLibrary.Tools;

namespace LilTestField;

class Program
{
    static void Main(string[] args)
    {
        var json = """
                   {
                       "data": {
                           "id": 1,
                           "type": "posts",
                           "attributes": {
                               "title": "Заголовок",
                               "content": "Содержимое",
                               "created_at": "2025-01-15T10:30:00+00:00",
                               "updated_at": "2025-01-15T10:30:00+00:00"
                           },
                           "relationships": {
                               "author": {
                                   "id": 1,
                                   "name": "Иван Петров"
                               }
                           },
                           "links": {
                               "self": "http://api.example.com/posts/1"
                           }
                       },
                       "status": 201,
                       "message": "message"
                   }
                   """;

        var test = Test<object>(json);
        ;
    }

    private static T? Test<T>(string json)
    {
        var test = JsonSerializer.Deserialize<LaravelJsonResponse>(json);

        if (test.Status == (HttpStatusCode)201 && test?.Data is not null)
        {
            return (T)test.Data;
        }
        ;
        return default;
    }
}