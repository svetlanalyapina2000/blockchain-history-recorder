using Cassandra;

namespace BlockchainHistoryRecorder.Domain;

public interface IRepository<T> where T : class, IEntity, IAuditableEntity
{
    Task<IEnumerable<T>> GetListByNameAsync(string name);

    Task<BoundStatement> AddAsync(T entity, CancellationToken cancellationToken = default);
}