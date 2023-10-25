using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PropCore.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation(
            "[{RequestName}] Handling request @ {Timestamp}",
            requestName,
            DateTime.UtcNow);

        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > 500)
        {
            logger.LogWarning(
                "[{RequestName}] took {Elapsed}ms to complete",
                requestName,
                stopwatch.ElapsedMilliseconds);
        }
        else
        {
            logger.LogInformation(
                "[{RequestName}] completed in {Elapsed}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}