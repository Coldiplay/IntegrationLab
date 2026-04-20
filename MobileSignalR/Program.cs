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
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddSignalR();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Some API v1", Version = "v1" });
            options.AddSignalRSwaggerGen();
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
            
        });
        builder.Services.AddOpenApi();
        
        builder.Services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                var xml = Options.RSA;
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
                    ValidIssuer = Options.Issuer,
                    ValidAudience = Options.Audience
                };
            });
        
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Authorized", policy => policy.Requirements.Add(
                    new AuthRequirement(new HttpClient {BaseAddress = new Uri(GlobalOptions.API_URI)})
                ));

        builder.Services.AddKeyedSingleton("ForMobileHub", new HttpClient {
            BaseAddress = new Uri(GlobalOptions.API_URI)
        });
        // ???
        // builder.Services.AddSingleton<HttpClient>(_ => new HttpClient {
        //     BaseAddress = new Uri(GlobalOptions.API_URI)
        // });

        builder.Services.AddSingleton<JwtTokenHandler>();
        builder.Services.AddHostedService(provider => provider.GetRequiredService<JwtTokenHandler>());
        
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
        app.MapControllers();

        app.UseMiddleware<GlobalExceptionMiddleWare>();
        app.Run();
    }
}