using BaseLibrary.Db;
using MobileSignalR.Hub;
using MobileSignalR.MiddleWares;

namespace MobileSignalR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        builder.Services.AddDbContext<IntegrationDbContext>();
        
        //new HttpClient().GetAsync("").Result.Content.
        var app = builder.Build();

        //app.MapGet("/", () => "Hello World!");

        app.MapHub<MobileHub>("/why");

        app.UseMiddleware<GlobalExceptionMiddleWare>();
        app.Run();
    }
}