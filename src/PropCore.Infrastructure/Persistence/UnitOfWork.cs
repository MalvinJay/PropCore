using PropCore.Application.Abstractions.Persistence;

namespace PropCore.Infrastructure.Persistence;

public sealed class UnitOfWork(PropCoreDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}