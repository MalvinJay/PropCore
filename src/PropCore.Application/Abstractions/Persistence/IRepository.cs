using PropCore.Domain.Common;

namespace PropCore.Application.Abstractions.Persistence;

public interface IRepository<T>
    where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}