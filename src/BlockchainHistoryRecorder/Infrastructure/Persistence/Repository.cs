using System.Reflection;
using BlockchainHistoryRecorder.Common.Extensions;
using BlockchainHistoryRecorder.Domain;
using Cassandra;
using Cassandra.Data.Linq;
using ISession = Cassandra.ISession;


namespace BlockchainHistoryRecorder.Infrastructure.Persistence;

public class Repository<T> : IRepository<T> where T : class, IEntity, IAuditableEntity
{
    private static readonly PropertyInfo[] _properties = typeof(T).GetProperties();
    private readonly ISession _session;

    public Repository(ISession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<T>> GetListByNameAsync(string name)
    {
        return await new Table<T>(_session).Where(x => x.Name == name).ExecuteAsync();
    }

    public virtual async Task<BoundStatement> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var properties = _properties.ExtractEntityProperties(entity);
        var query = $"INSERT INTO {entity.GetType().Name} ({properties.columnNames}) VALUES ({properties.bindMarkers})";
        var preparedStatement = await _session.PrepareAsync(query);
        return preparedStatement.Bind(properties.propertiesValue);
    }
}