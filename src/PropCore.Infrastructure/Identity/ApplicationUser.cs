using Microsoft.AspNetCore.Identity;

namespace PropCore.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid OrganizationId { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}