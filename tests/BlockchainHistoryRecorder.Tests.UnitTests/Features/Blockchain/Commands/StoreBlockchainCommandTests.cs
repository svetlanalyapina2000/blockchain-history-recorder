using AutoFixture;
using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;
using Cassandra;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BlockchainHistoryRecorder.Tests.UnitTests.Features.Blockchain.Commands;

public class StoreBlockchainCommandTests
{
    private readonly Fixture _fixture = new();
    private readonly StoreBlockchainHandler _handler;
    private readonly IMapper _mapperMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public StoreBlockchainCommandTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _mapperMock = Substitute.For<IMapper>();
        var mockLogger = Substitute.For<ILogger<StoreBlockchainHandler>>();
        _handler = new StoreBlockchainHandler(_mapperMock, _unitOfWorkMock, mockLogger);
    }

    [Fact]
    public async Task Handle_NormalFlow()
    {
        var command = new StoreBlockchainCommand { Blockchain = _fixture.Create<BlockchainData>() };
        var blockchain = _fixture.Create<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain>();
        var statement = Substitute.For<BoundStatement>();
        _mapperMock.Map<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain>(command.Blockchain)
            .Returns(blockchain);
        _unitOfWorkMock.Blockchains.AddAsync(blockchain, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(statement));

        await _handler.Handle(command, CancellationToken.None);

        _mapperMock.Received(1)
            .Map<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain>(command.Blockchain);
        await _unitOfWorkMock.Blockchains.Received(1).AddAsync(blockchain, Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Is<List<Statement>>(list => list.Contains(statement)));
    }
}