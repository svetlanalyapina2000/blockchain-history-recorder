using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Tests.Features.Blockchain.TestFactories;
using BlockchainHistoryRecorder.Tests.Integration.Abstractions;
using BlockchainHistoryRecorder.Tests.Integration.Features.Blockchain.TestData;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainHistoryRecorder.Tests.Integration.Features.Blockchain.Queries;

public class GetBlockchainHistoryHandlerTests : BaseTests
{
    #region Entities

    private readonly List<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain> _blockchains = [];

    #endregion

    public GetBlockchainHistoryHandlerTests(TestWebApplicationFactory factory) : base(factory)
    {
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.BitcoinMainnet));
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.BitcoinTestnet));
        _blockchains.Add(
            BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper, BlockchainIdentifier.DashMainnet));
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.LitecoinMainnet));
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.EthereumMainnet));
    }

    [Theory]
    [MemberData(nameof(BlockchainIdentifierTestData.BlockchainIdentifiers),
        MemberType = typeof(BlockchainIdentifierTestData))]
    public async Task HandleGetBlockchainHistory_NormalFlow(BlockchainIdentifier blockchainIdentifier)
    {
        var handler = Scope.ServiceProvider
            .GetRequiredService<IRequestHandler<GetBlockchainHistoryQuery, GetBlockchainHistoryResponse>>();
        var request = new GetBlockchainHistoryQuery { BlockchainIdentifier = blockchainIdentifier };

        var response = await handler.Handle(request, new CancellationToken());

        response.Should().NotBeNull();
        response.BlockchainHistory.Should().HaveCount(1);
        var blockchain = response.BlockchainHistory.First();
        var testBlockchain = _blockchains.First(x => x.Name == blockchain.Name);
        blockchain.Name.Should().Be(testBlockchain.Name);
        blockchain.Hash.Should().Be(testBlockchain.Hash);
        blockchain.Height.Should().Be(testBlockchain.Height);
        blockchain.Time.Should().BeCloseTo(testBlockchain.Time, TimeSpan.FromMilliseconds(1));
        blockchain.LatestUrl.Should().Be(testBlockchain.LatestUrl);
        blockchain.PreviousHash.Should().Be(testBlockchain.PreviousHash);
        blockchain.PreviousUrl.Should().Be(testBlockchain.PreviousUrl);
        blockchain.PeerCount.Should().Be(testBlockchain.PeerCount);
        blockchain.UnconfirmedCount.Should().Be(testBlockchain.UnconfirmedCount);
        blockchain.HighFeePerKb.Should().Be(testBlockchain.HighFeePerKb);
        blockchain.MediumFeePerKb.Should().Be(testBlockchain.MediumFeePerKb);
        blockchain.LowFeePerKb.Should().Be(testBlockchain.LowFeePerKb);
        blockchain.HighGasPrice.Should().Be(testBlockchain.HighGasPrice);
        blockchain.MediumGasPrice.Should().Be(testBlockchain.MediumGasPrice);
        blockchain.LowGasPrice.Should().Be(testBlockchain.LowGasPrice);
        blockchain.LastForkHeight.Should().Be(testBlockchain.LastForkHeight);
        blockchain.LastForkHash.Should().Be(testBlockchain.LastForkHash);
        blockchain.CreatedAt.Should().BeCloseTo(testBlockchain.CreatedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task HandleGetBlockchainHistory_UnknownIdentifier_ThrowArgumentException()
    {
        var handler = Scope.ServiceProvider
            .GetRequiredService<IRequestHandler<GetBlockchainHistoryQuery, GetBlockchainHistoryResponse>>();
        var request = new GetBlockchainHistoryQuery { BlockchainIdentifier = BlockchainIdentifier.Unknown };

        Func<Task> act = async () => await handler.Handle(request, new CancellationToken());

        await act.Should().ThrowAsync<ArgumentException>();
    }


    protected override void Cleanup()
    {
        foreach (var blockchain in _blockchains)
            BlockchainTestFactory.DeleteBlockchainRecord(Session, blockchain.Name, blockchain.CreatedAt);
    }
}