using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PropCore.Application.Common.Behaviors;

namespace PropCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(ValidationBehavior<,>));

        return services;
    }
}