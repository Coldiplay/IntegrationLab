using System.Diagnostics;
using System.Net;
using System.Text.Json;
using BaseLibrary.Model;
using BaseLibrary.Tools;

namespace LilTestField;

class Program
{
    static void Main(string[] args)
    {
        var json = """
                   {
                       "type": "Гдетотам\\PostResource",
                       "data": {
                           "id": 1,
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
                        },
                       "status": 201,
                       "message": "message"
                   }
                   """;


        var sw = new Stopwatch();
        sw.Start();
        var test = LaravelParser.ParseResponse<Post>(json);
        sw.Stop();
        var tess = sw.ElapsedMilliseconds;
        ;
        sw.Reset();
        sw.Start();
        var test2 = LaravelParser.ParseResponse<Post>(json);
        sw.Stop();
        var tesss = sw.ElapsedMilliseconds;
        ;
    }

    
    
}