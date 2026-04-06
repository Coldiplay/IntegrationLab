using BaseLibrary.Db;
using BaseLibrary.Tools;
using MobileSignalR.Hub;
using MobileSignalR.MiddleWares;

namespace MobileSignalR;

public class Program
{
    // private static readonly HttpClient HttpApi = new() {
    //     BaseAddress = new Uri(GlobalOptions.API_URI)
    // };
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.Services.AddAuthentication().AddScheme<>();
        builder.Services.AddSignalR();
        builder.Services.AddDbContext<IntegrationDbContext>();
        builder.Services.AddSingleton<HttpClient>(opt => new HttpClient {
            BaseAddress = new Uri(GlobalOptions.API_URI)
        });
        
        //new HttpClient().GetAsync("").Result.Content.
        var app = builder.Build();

        //app.MapGet("/", () => "Hello World!");

        app.MapHub<MobileHub>("/hub");

        app.UseMiddleware<GlobalExceptionMiddleWare>();
        app.Run();
    }
}