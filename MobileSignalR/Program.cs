using System.Net;
using BaseLibrary.Db;
using BaseLibrary.Tools;
using Microsoft.OpenApi;
using MobileSignalR.Auth;
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

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Some API v1", Version = "v1" });
            options.AddSignalRSwaggerGen();
        });
        builder.Services.AddOpenApi();
        
        //Надеюсь, сработает
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Authorized", policy => policy.Requirements.Add(
                new AuthRequirement(new HttpClient {BaseAddress = new Uri(GlobalOptions.API_URI)})
                ));
        });
        
        builder.Services.AddSignalR();
        builder.Services.AddDbContext<IntegrationDbContext>();
        builder.Services.AddSingleton<HttpClient>(_ => new HttpClient {
            BaseAddress = new Uri(GlobalOptions.API_URI)
        });

        
        
        //new HttpClient().GetAsync("").Result.Content.
        var app = builder.Build();
        
        if (builder.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<MobileHub>("/hub");

        app.UseMiddleware<GlobalExceptionMiddleWare>();
        app.Run();
    }
}