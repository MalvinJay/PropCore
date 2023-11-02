using Microsoft.AspNetCore.Diagnostics;
using PropCore.Application.Common.Exceptions;
using PropCore.Domain.Exceptions;

namespace PropCore.Api.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problem = new Microsoft.AspNetCore.Mvc.ValidationProblemDetails(exception.Errors)
            {
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest
            };

            await context.Response.WriteAsJsonAsync(
                problem,
                problem.GetType(),
                context.RequestAborted);
        }
        catch (DomainException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "Domain Rule Violation",
                Status = StatusCodes.Status409Conflict,
                Detail = exception.Message
            };

            await context.Response.WriteAsJsonAsync(
                problem,
                problem.GetType(),
                context.RequestAborted);
        }
        catch (InvalidOperationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "Invalid State Transition",
                Status = StatusCodes.Status409Conflict,
                Detail = exception.Message
            };

            await context.Response.WriteAsJsonAsync(
                problem,
                problem.GetType(),
                context.RequestAborted);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception for request {RequestPath}",
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "An error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsJsonAsync(
                problem,
                problem.GetType(),
                context.RequestAborted);
        }
    }
}