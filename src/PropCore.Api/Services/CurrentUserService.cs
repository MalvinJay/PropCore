using System.Security.Claims;
using PropCore.Application.Abstractions.Authentication;

namespace PropCore.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public Guid? OrganizationId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue("organizationId");

            return Guid.TryParse(value, out var organizationId) ? organizationId : null;
        }
    }

    public string? Email =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public string? IpAddress
    {
        get
        {
            var context = httpContextAccessor.HttpContext;

            if (context is null)
            {
                return null;
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
}