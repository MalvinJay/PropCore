using System.Text.Json;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PropCore.Api.Middleware;
using PropCore.Api.Services;
using PropCore.Application;
using PropCore.Application.Abstractions.Authentication;
using PropCore.Infrastructure;
using PropCore.Infrastructure.Persistence;
using RabbitMQ.Client;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

    builder.Services.AddHealthChecks()
        .AddSqlServer(
            builder.Configuration.GetConnectionString("PropCore")!,
            name: "sqlserver")
        .AddRedis(
            builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379",
            name: "redis")
        .AddRabbitMQ(sp =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(GetRabbitConnectionString(builder.Configuration))
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, CurrentUserService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
            policy
                .WithOrigins(builder.Configuration["Cors:AllowedOrigins"]?.Split(';') ?? ["http://localhost:5173"])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PropCore API",
                Version = "v1",
                Description = "Enterprise property management platform API."
            });
        });
    }

    var app = builder.Build();

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseCors("Frontend");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PropCore API v1");
        });
    }

    app.MapControllers();

    app.MapHealthChecks("/api/v1/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PropCoreDbContext>();

    if (builder.Environment.IsDevelopment() && db.Database.IsSqlServer())
    {
        db.Database.Migrate();
    }

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "PropCore.Api terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}

static string GetRabbitConnectionString(IConfiguration configuration)
{
    var host = configuration["RabbitMQ:Host"]
        ?? throw new InvalidOperationException("RabbitMQ:Host is not configured.");
    var port = configuration["RabbitMQ:Port"]
        ?? throw new InvalidOperationException("RabbitMQ:Port is not configured.");
    var username = configuration["RabbitMQ:Username"]
        ?? throw new InvalidOperationException("RabbitMQ:Username is not configured.");
    var password = configuration["RabbitMQ:Password"]
        ?? throw new InvalidOperationException("RabbitMQ:Password is not configured.");
    var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";

    return $"amqp://{username}:{password}@{host}:{port}/{virtualHost}";
}

public partial class Program
{
}