using BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;
using Cassandra;
using ISession = Cassandra.ISession;

namespace BlockchainHistoryRecorder.Features.Blockchains.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ISession _session;

    public UnitOfWork(ISession session)
    {
        _session = session;
        Blockchains = new BlockchainRepository(_session);
    }

    public IBlockchainRepository Blockchains { get; }

    public async Task CompleteAsync(List<Statement> queries)
    {
        var batchStatement = new BatchStatement();
        foreach (var query in queries) batchStatement.Add(query);

        await _session.ExecuteAsync(batchStatement);
    }
}