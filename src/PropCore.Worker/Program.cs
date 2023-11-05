using Serilog;
using PropCore.Application;
using PropCore.Infrastructure;
using PropCore.Worker.Services;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build())
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting PropCore.Worker");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    builder.Services.AddHostedService<OutboxProcessorWorker>();

    var host = builder.Build();

    await host.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "PropCore.Worker terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}