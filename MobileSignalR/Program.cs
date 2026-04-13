using BaseLibrary.Db;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
        builder.Services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //private key?
                const string xml = "<RSAKeyValue> load...from..local...files...</RSAKeyValue>";
                var key = KeyHelper.BuildRsaSigningKey(xml);

                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.IncludeErrorDetails = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = JwtOptions.Issuer,
                    ValidAudience = JwtOptions.Audience
                };
            });
        
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Authorized", policy => policy.Requirements.Add(
                    new AuthRequirement(new HttpClient {BaseAddress = new Uri(GlobalOptions.API_URI)})
                ));
        
        builder.Services.AddSignalR();
        builder.Services.AddDbContext<IntegrationDbContext>();
        builder.Services.AddSingleton<HttpClient>(_ => new HttpClient {
            BaseAddress = new Uri(GlobalOptions.API_URI)
        });
        
        
        //new HttpClient().GetAsync("").Result.Content.
        var app = builder.Build();
        
        app.UseHttpsRedirection();
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