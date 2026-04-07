using System.Diagnostics;
using System.Net;
using System.Text.Json;
using BaseLibrary.Model;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.SignalR.Client;

namespace LilTestField;

class Program
{
    static async Task Main(string[] args)
    {
        /*
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
        */
        
        var connection = new HubConnectionBuilder().WithUrl("https://localhost:7045/hubs/MobileHub").WithAutomaticReconnect().Build();
        //connection.SendAsync("Authorize", "test", "test2").Wait();
        await connection.StartAsync();
        var response = await connection.InvokeAsync<object>("Authorize", "test", "test2");
        ;
    }

    
    
}