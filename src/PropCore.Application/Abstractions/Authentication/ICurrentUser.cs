namespace PropCore.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    Guid? UserId { get; }
    Guid? OrganizationId { get; }
    string? Email { get; }
    string? IpAddress { get; }
    bool IsAuthenticated { get; }
}