using Microsoft.EntityFrameworkCore;
using PropCore.Application.Abstractions.Persistence;
using PropCore.Domain.Common;

namespace PropCore.Infrastructure.Persistence.Repositories;

public sealed class GenericRepository<T>(PropCoreDbContext context) : IRepository<T>
    where T : Entity
{
    private DbSet<T> Entities => context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Entities.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        return await Entities.Where(e => idList.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    public void Add(T entity)
    {
        Entities.Add(entity);
    }

    public void Update(T entity)
    {
        Entities.Update(entity);
    }

    public void Remove(T entity)
    {
        Entities.Remove(entity);
    }
}