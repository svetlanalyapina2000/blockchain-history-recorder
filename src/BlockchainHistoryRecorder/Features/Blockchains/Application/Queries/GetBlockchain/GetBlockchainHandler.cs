using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;
using MediatR;
using Abstraction_IBlockcypherHttpClient = BlockchainHistoryRecorder.Application.Abstraction.IBlockcypherHttpClient;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;

public class GetBlockchainHandler : IRequestHandler<GetBlockchainQuery, GetBlockchainResponse?>
{
    private readonly Abstraction_IBlockcypherHttpClient _blockcypherHttpClient;
    private readonly ILogger<GetBlockchainHandler> _logger;
    private readonly IMapper _mapper;


    public GetBlockchainHandler(Abstraction_IBlockcypherHttpClient blockcypherHttpClient, IMapper mapper,
        ILogger<GetBlockchainHandler> logger)
    {
        _blockcypherHttpClient = blockcypherHttpClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetBlockchainResponse?> Handle(GetBlockchainQuery request, CancellationToken token)
    {
        var blockchainName = GetBlockchainMapper.GetStringFromEnum(request.BlockchainIdentifier);
        if (blockchainName is null)
            throw new ArgumentException(
                $"Cannot find blockchain with such name. Name : {request.BlockchainIdentifier.ToString()}");
        var latestBlockchainInfo = await _blockcypherHttpClient.GetLatestBlockchainAsync(blockchainName, token);
        if (latestBlockchainInfo is null)
        {
            _logger.LogWarning("No data returned for blockchain {BlockchainName}", blockchainName);
            return null;
        }

        _logger.LogInformation("Retrieved latest blockchain info for {BlockchainName}", blockchainName);
        return new GetBlockchainResponse { Blockchain = _mapper.Map<BlockchainData>(latestBlockchainInfo) };
    }
}