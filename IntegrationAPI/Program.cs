using IntegrationAPI.Db;
using IntegrationAPI.MiddleWares;
using BaseLibrary.Tools;

namespace IntegrationAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<IntegrationDbContext>();
        builder.Services.AddSignalR();
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy =
                GlobalOptions.JsonSerializerOptions.PropertyNamingPolicy;
            options.JsonSerializerOptions.ReferenceHandler =
                GlobalOptions.JsonSerializerOptions.ReferenceHandler;
        });
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<GlobalExceptionMiddleWare>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        
        app.Run();
    }
}