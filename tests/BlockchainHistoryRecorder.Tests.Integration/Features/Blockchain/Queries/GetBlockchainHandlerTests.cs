using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Tests.Integration.Abstractions;
using BlockchainHistoryRecorder.Tests.Integration.Features.Blockchain.TestData;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainHistoryRecorder.Tests.Integration.Features.Blockchain.Queries;

public class GetBlockchainHandlerTests : BaseTests
{
    public GetBlockchainHandlerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Theory]
    [MemberData(nameof(BlockchainIdentifierTestData.BlockchainIdentifiers),
        MemberType = typeof(BlockchainIdentifierTestData))]
    public async Task HandleGetBlockchain_NormalFlow(BlockchainIdentifier blockchainIdentifier)
    {
        var handler = Scope.ServiceProvider
            .GetRequiredService<IRequestHandler<GetBlockchainQuery, GetBlockchainResponse>>();
        var request = new GetBlockchainQuery { BlockchainIdentifier = blockchainIdentifier };

        var response = await handler.Handle(request, new CancellationToken());

        var blockchain = response.Blockchain;
        response.Should().NotBeNull();
        blockchain.Name.Should().NotBeNullOrWhiteSpace();
        blockchain.Height.Should().BeGreaterThan(0);
        blockchain.Hash.Should().NotBeNullOrWhiteSpace();
        blockchain.Time.Should().BeBefore(DateTime.UtcNow.Add(TimeSpan.FromSeconds(1)));
        blockchain.LatestUrl.Should().NotBeNullOrWhiteSpace();
        blockchain.PeerCount.Should().BeGreaterOrEqualTo(0);
        blockchain.UnconfirmedCount.Should().BeGreaterOrEqualTo(0);
        blockchain.LastForkHeight.Should().Match(height => !height.HasValue || height > 0);
    }

    [Fact]
    public async Task HandleGetBlockchain_UnknownIdentifier_ThrowArgumentException()
    {
        var handler = Scope.ServiceProvider
            .GetRequiredService<IRequestHandler<GetBlockchainQuery, GetBlockchainResponse>>();
        var request = new GetBlockchainQuery { BlockchainIdentifier = BlockchainIdentifier.Unknown };

        Func<Task> act = async () => await handler.Handle(request, new CancellationToken());

        await act.Should().ThrowAsync<ArgumentException>();
    }
}