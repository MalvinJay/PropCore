using PropCore.Domain.Common;
using PropCore.Domain.Enums;

namespace PropCore.Domain.Entities;

public sealed class Tenant : AggregateRoot
{
    private Tenant()
    {
    }

    public Guid OrganizationId { get; private set; }
    public Guid? UserId { get; private set; }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }

    public TenantStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Tenant Create(
        Guid organizationId,
        string firstName,
        string lastName,
        string email,
        string? phone,
        Guid? userId = null)
    {
        return new Tenant
        {
            OrganizationId = organizationId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            UserId = userId,
            Status = TenantStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string? phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkUser(Guid userId)
    {
        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFormer()
    {
        Status = TenantStatus.Former;
        UpdatedAt = DateTime.UtcNow;
    }
}