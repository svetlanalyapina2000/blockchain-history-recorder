using BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Infrastructure.Persistence;
using ISession = Cassandra.ISession;


namespace BlockchainHistoryRecorder.Features.Blockchains.Infrastructure.Persistence;

public class BlockchainRepository : Repository<Blockchain>, IBlockchainRepository
{
    public BlockchainRepository(ISession session) : base(session)
    {
    }
}