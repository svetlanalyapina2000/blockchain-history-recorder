using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;
using MediatR;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;

public class GetBlockchainHistoryHandler : IRequestHandler<GetBlockchainHistoryQuery, GetBlockchainHistoryResponse>
{
    private readonly ILogger<GetBlockchainHistoryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetBlockchainHistoryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<GetBlockchainHistoryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetBlockchainHistoryResponse> Handle(GetBlockchainHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var blockchainName = GetBlockchainHistoryMapper.GetStringFromEnum(request.BlockchainIdentifier);
        if (blockchainName is null)
            throw new ArgumentException(
                $"Cannot find blockchain with such name. Name : {request.BlockchainIdentifier.ToString()}");
        var blockchains = await _unitOfWork.Blockchains.GetListByNameAsync(blockchainName);
        _logger.LogInformation("Retrieved blockchain records for {BlockchainName}", blockchainName);
        return new GetBlockchainHistoryResponse
        {
            BlockchainHistory =
                _mapper.Map<List<GetBlockchainHistoryResponse.BlockchainResponse>>(blockchains)
        };
    }
}