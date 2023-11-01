using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PropCore.Application.Abstractions.Caching;
using PropCore.Application.Abstractions.Messaging;
using PropCore.Application.Abstractions.Persistence;
using PropCore.Application.Abstractions.Storage;
using PropCore.Application.Abstractions.Notifications;
using PropCore.Infrastructure.Caching;
using PropCore.Infrastructure.Messaging;
using PropCore.Infrastructure.Notifications;
using PropCore.Infrastructure.Persistence;
using PropCore.Infrastructure.Persistence.Repositories;
using PropCore.Infrastructure.Storage;

namespace PropCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PropCore")
            ?? throw new InvalidOperationException(
                "Connection string 'PropCore' is not configured.");

        services.AddDbContext<PropCoreDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddDbContextFactory<PropCoreDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();

        services.AddScoped<Messaging.Outbox.OutboxProcessor>();

        AddCaching(services, configuration);
        AddStorage(services, configuration);
        AddMessaging(services, configuration);

        services.AddSingleton<INotificationService, InMemoryNotificationService>();

        return services;
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration["Redis:ConnectionString"];

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "propcore:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<ICacheService, CacheService>();
    }

    private static void AddStorage(IServiceCollection services, IConfiguration configuration)
    {
        var storageRoot = configuration["Storage:RootPath"] ?? "uploads";

        services.AddSingleton<IStorageService>(provider =>
            new FileSystemStorageService(
                provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<FileSystemStorageService>>(),
                storageRoot));
    }

    private static void AddMessaging(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqTransportOptions>()
            .Bind(configuration.GetSection("RabbitMQ"));

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqTransportOptions>>();

                cfg.Host(options.Value.Host, options.Value.Port, options.Value.VHost, h =>
                {
                    h.Username(options.Value.User);
                    h.Password(options.Value.Pass);
                });

                cfg.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}