using BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Tests.Features.Blockchain.TestFactories;
using BlockchainHistoryRecorder.Tests.Integration.Abstractions;
using BlockchainHistoryRecorder.Tests.Integration.Features.Blockchain.TestData;
using Cassandra;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainHistoryRecorder.Tests.Integration.Features.Blockchain.Commands;

public class StoreBlockchainHandlerTests : BaseTests
{
    public StoreBlockchainHandlerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Theory]
    [MemberData(nameof(BlockchainIdentifierTestData.BlockchainIdentifiers),
        MemberType = typeof(BlockchainIdentifierTestData))]
    public async Task HandleStoreBlockchain_NormalFlow(BlockchainIdentifier blockchainIdentifier)
    {
        var testBlockchain = BlockchainTestFactory.CreateBlockchainData(Fixture, blockchainIdentifier);
        var handler = Scope.ServiceProvider
            .GetRequiredService<IRequestHandler<StoreBlockchainCommand, StoreBlockchainResponse>>();
        var request = new StoreBlockchainCommand { Blockchain = testBlockchain };

        var blockchain = await handler.Handle(request, new CancellationToken());

        blockchain.Name.Should().Be(testBlockchain.Name);
        blockchain.Hash.Should().Be(testBlockchain.Hash);
        blockchain.Height.Should().Be(testBlockchain.Height);
        blockchain.Time.Should().BeCloseTo(testBlockchain.Time, TimeSpan.FromMilliseconds(1));
        blockchain.LatestUrl.Should().Be(testBlockchain.LatestUrl);
        blockchain.PreviousHash.Should().Be(testBlockchain.PreviousHash);
        blockchain.PreviousUrl.Should().Be(testBlockchain.PreviousUrl);
        blockchain.PeerCount.Should().Be(testBlockchain.PeerCount);
        blockchain.UnconfirmedCount.Should().Be(testBlockchain.UnconfirmedCount);
        blockchain.LastForkHeight.Should().Be(testBlockchain.LastForkHeight);
        blockchain.LastForkHash.Should().Be(testBlockchain.LastForkHash);
        blockchain.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);

        //Cleanup
        BlockchainTestFactory.DeleteBlockchainRecord(Session, blockchain.Name, blockchain.CreatedAt);
    }

    [Fact]
    public async Task HandleFetchBlockchain_EmptyName_ThrowArgumentException()
    {
        var testBlockchain = BlockchainTestFactory.CreateBlockchainDataWithEmptyName(Fixture);
        var handler = Scope.ServiceProvider
            .GetRequiredService<IRequestHandler<StoreBlockchainCommand, StoreBlockchainResponse>>();
        var request = new StoreBlockchainCommand { Blockchain = testBlockchain };

        Func<Task> act = async () => await handler.Handle(request, new CancellationToken());

        await act.Should().ThrowAsync<InvalidQueryException>();
    }
}