using AutoFixture;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace BlockchainHistoryRecorder.Tests.Features.Blockchain.TestFactories;

public class BlockchainTestFactory
{
    public static void DeleteBlockchainRecord(ISession session, string name, DateTime createdAt)
    {
        new Table<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain>(session)
            .Where(u => u.Name == name && u.CreatedAt == createdAt)
            .Delete()
            .Execute();
    }

    public static BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain CreateBlockchainRecord(
        IFixture fixture, IMapper mapper, BlockchainIdentifier blockchainIdentifier)
    {
        var name = GetBlockchainHistoryMapper.GetStringFromEnum(blockchainIdentifier);
        var blockchain = fixture.Build<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain>()
            .With(x => x.Name, name)
            .Create();
        mapper.Insert(blockchain);
        return blockchain;
    }

    public static BlockchainData CreateBlockchainDataWithEmptyName(IFixture fixture)
    {
        var blockchain = fixture.Build<BlockchainData>()
            .With(x => x.Name, string.Empty)
            .Create();
        return blockchain;
    }

    public static BlockchainData CreateBlockchainData(IFixture fixture, BlockchainIdentifier blockchainIdentifier)
    {
        var name = GetBlockchainHistoryMapper.GetStringFromEnum(blockchainIdentifier);
        var blockchain = fixture.Build<BlockchainData>()
            .With(x => x.Name, name)
            .Create();
        return blockchain;
    }
}