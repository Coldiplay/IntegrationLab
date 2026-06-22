using System.Net.Http.Headers;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MobileSignalR.Auth;
using MobileSignalR.Hub;
using MobileSignalR.MiddleWares;
using MobileSignalR.Notifications.Events;
using MobileSignalR.Notifications.Handlers;
using MobileSignalR.Tools;
using RabbitMQ.Client;

namespace MobileSignalR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMemoryCache(); //TODO: Потом поставить size limit

        builder.Services.AddControllers();
        builder.Services.AddSignalR();

        builder.Services.AddSwaggerGen(options => {
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

        builder.Services.AddAuthentication(a => {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o => {
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
                new AuthRequirement(new HttpClient { BaseAddress = new Uri(GlobalOptions.API_URI) })
            ));

        builder.Services.AddSingleton(_ => {
            var client = new HttpClient
            {
                BaseAddress = new Uri(GlobalOptions.API_URI),
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        });

        builder.Services.AddSingleton<LaravelRequestHandler>();
        builder.Services.AddSingleton<JwtTokenHandler>();
        builder.Services.AddSingleton<ConnectionsHandler>();
        builder.Services.AddSingleton<IConnection>(new ConnectionFactory() {
            HostName = RabbitMqConsumerOptions.HostName,
            VirtualHost = RabbitMqConsumerOptions.VirtualHost,
            UserName = RabbitMqConsumerOptions.UserName,
            Password = RabbitMqConsumerOptions.Password,
            AutomaticRecoveryEnabled = true,         // авто-переподключение
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            TopologyRecoveryEnabled = RabbitMqConsumerOptions.DeclareTopology // если true — попытается восстановить биндинги
        }.CreateConnectionAsync().Result);
        
        builder.Services.AddHostedService(provider => provider.GetRequiredService<JwtTokenHandler>());

        RegisterNotificationHandlers(builder.Services);
        
        builder.Services.AddSingleton<EventDispatcher>();
        builder.Services.AddHostedService<RabbitMqEventConsumer>();
        
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


    private static void RegisterNotificationHandlers(IServiceCollection services)
    {
        services.AddSingleton<IEventHandler, MessageEventHandler>();
        services.AddSingleton<IEventHandler, ChatEventHandler>();
        services.AddSingleton<IEventHandler, IncidentEventHandler>();
        services.AddSingleton<IEventHandler, ShippingEventHandler>();
    }
}