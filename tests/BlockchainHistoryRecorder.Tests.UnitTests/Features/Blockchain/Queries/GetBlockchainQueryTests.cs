using AutoFixture;
using AutoMapper;
using BlockchainHistoryRecorder.Application.Abstraction;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Features.Blockchains.Infrastructure.BlockchainHttpClient;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BlockchainHistoryRecorder.Tests.UnitTests.Features.Blockchain.Queries;

public class GetBlockchainQueryTests
{
    private readonly Fixture _fixture = new();
    private readonly GetBlockchainHandler _handler;
    private readonly IBlockcypherHttpClient _httpClient;
    private readonly IMapper _mapperMock;


    public GetBlockchainQueryTests()
    {
        _httpClient = Substitute.For<IBlockcypherHttpClient>();
        _mapperMock = Substitute.For<IMapper>();
        var mockLogger = Substitute.For<ILogger<GetBlockchainHandler>>();
        _handler = new GetBlockchainHandler(_httpClient, _mapperMock, mockLogger);
    }

    [Fact]
    public async Task Handle_NormalFlow()
    {
        var testName = "btc/main";
        var command = new GetBlockchainQuery { BlockchainIdentifier = BlockchainIdentifier.BitcoinMainnet };
        var blockchainDTO = _fixture.Create<BlockchainDTO?>();
        var blockchainData = _fixture.Create<BlockchainData>();
        _mapperMock.Map<BlockchainData>(blockchainDTO).Returns(blockchainData);
        _httpClient.GetLatestBlockchainAsync(testName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(blockchainDTO));

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        _mapperMock.Received(1).Map<BlockchainData>(blockchainDTO);
        response?.Blockchain.Should().BeEquivalentTo(blockchainData);
        await _httpClient.Received(1).GetLatestBlockchainAsync(testName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownIdentifier_ReturnArgumentException()
    {
        var request = new GetBlockchainQuery { BlockchainIdentifier = BlockchainIdentifier.Unknown };

        var act = async () => { await _handler.Handle(request, CancellationToken.None); };

        await act.Should().ThrowAsync<ArgumentException>();
    }
}