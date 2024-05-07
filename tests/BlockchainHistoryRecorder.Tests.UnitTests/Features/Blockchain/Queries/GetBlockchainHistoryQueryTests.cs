using AutoFixture;
using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BlockchainHistoryRecorder.Tests.UnitTests.Features.Blockchain.Queries;

public class GetBlockchainHistoryQueryTests
{
    private readonly Fixture _fixture = new();
    private readonly GetBlockchainHistoryHandler _handler;
    private readonly IMapper _mapperMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public GetBlockchainHistoryQueryTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _mapperMock = Substitute.For<IMapper>();
        var mockLogger = Substitute.For<ILogger<GetBlockchainHistoryHandler>>();
        _handler = new GetBlockchainHistoryHandler(_unitOfWorkMock, _mapperMock, mockLogger);
    }

    [Fact]
    public async Task Handle_NormalFlow()
    {
        var blockchainName = "BTC.main";
        var request = new GetBlockchainHistoryQuery { BlockchainIdentifier = BlockchainIdentifier.BitcoinMainnet };
        var blockchains =
            _fixture.Create<IEnumerable<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain>>();
        var blockchainResponses = _fixture.Create<List<GetBlockchainHistoryResponse.BlockchainResponse>>();
        _unitOfWorkMock.Blockchains.GetListByNameAsync(blockchainName).Returns(Task.FromResult(blockchains));
        _mapperMock.Map<List<GetBlockchainHistoryResponse.BlockchainResponse>>(blockchains)
            .Returns(blockchainResponses);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.BlockchainHistory.Should().BeEquivalentTo(blockchainResponses);
        await _unitOfWorkMock.Blockchains.Received(1).GetListByNameAsync(blockchainName);
        _mapperMock.Received(1).Map<List<GetBlockchainHistoryResponse.BlockchainResponse>>(blockchains);
    }

    [Fact]
    public async Task Handle_UnknownIdentifier_ThrowsArgumentException()
    {
        var request = new GetBlockchainHistoryQuery { BlockchainIdentifier = BlockchainIdentifier.Unknown };

        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }
}