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

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Some API v1", Version = "v1" });
            options.AddSignalRSwaggerGen();
            options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });
            
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
                //var xml = File.ReadAllText("private.xml");
                var xml = "<RSAKeyValue><Modulus>uq+6W9nJslsaLMdfcuHTtxuFk7RPmvj4NKfXTVm5a0hckqlTi5EEEltlaFE33X/8V2dQIql8vOvRDJo4z+/zlBaBLyRo2oliRBZudrc1vQI9euvai0kL16FH8YY6dYnbah9RhfdpNkhQfd4bZksfgvEOYSbxw/28K64j6i/tJ9U=</Modulus><Exponent>AQAB</Exponent><P>3UKsCIN8Kg+lQTtiBh+ExZddY92WiiP8XwlVaVLcC2EhKBktfdVFgdZKakEugWj+KebybDrRp1jqTVGzcUPNGw==</P><Q>1/9kCsFO1aNG0rOkrY/OOnhBH6Shww4xFzWxd00enQTXQ7CL5GvVobXJDbKsAp0dt8EkDHLbOPEHzYTdCL7dzw==</Q><DP>NM5Jsop24q7zOLtMbLuu+11hq4jh+bwW6jOXD9j3rTuUJzbDFaoFubQD9JHz4GzHZAa7SrtK+A6PdL6P/fM5iw==</DP><DQ>rDTHc/OugJFOc8oZru6KAv/BHBNLjJGR/ekm9fCcSZ+EaEknHxQCHI0sICmlDehpuwjXTr17nig8ilQ1TTWu7Q==</DQ><InverseQ>Y7KECd58t6Aef2JxkZTMd2EFhmiJdFHh3u4i/XPR8yG+zbO8liIqY/YFpy6ev1JnnG1dSAjfYsuRQPnR7vwg4g==</InverseQ><D>C8sGDr9XSnkO0j1V/j/dy/dlHMuLK9MGeu0PYMeGOwy7LFid+ncStsYnRcu7p7ZqDmtsWIQ0aQrMjetAI4KY9GpIPoIqsQCVnw2pVz4hw5iXpRDCY5udSG6nhYsokxKjBKe+sGHLwsszzTTe0qmlJSmH4LfZfUxS5ugzPrOLOJE=</D></RSAKeyValue>";
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
        
        // builder.Services.AddAuthorizationBuilder()
        //     .AddPolicy("Authorized", policy => policy.Requirements.Add(
        //             new AuthRequirement(new HttpClient {BaseAddress = new Uri(GlobalOptions.API_URI)})
        //         ));
        
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
        app.MapControllers();

        app.UseMiddleware<GlobalExceptionMiddleWare>();
        app.Run();
    }
}