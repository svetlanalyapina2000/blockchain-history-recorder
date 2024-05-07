using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using MediatR;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;

public class StoreBlockchainHandler : IRequestHandler<StoreBlockchainCommand, StoreBlockchainResponse>
{
    private readonly ILogger<StoreBlockchainHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;


    public StoreBlockchainHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<StoreBlockchainHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StoreBlockchainResponse> Handle(StoreBlockchainCommand command, CancellationToken token)
    {
        var blockchain = _mapper.Map<Blockchain>(command.Blockchain);
        var statement = await _unitOfWork.Blockchains.AddAsync(blockchain, token);
        await _unitOfWork.CompleteAsync([statement]);
        _logger.LogInformation("Blockchain added to the database with command {BlockchainCommand}.", command);
        return _mapper.Map<StoreBlockchainResponse>(blockchain);
    }
}