using Cassandra;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;

public interface IUnitOfWork
{
    IBlockchainRepository Blockchains { get; }
    Task CompleteAsync(List<Statement> queries);
}