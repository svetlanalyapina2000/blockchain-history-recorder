using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using MediatR;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;

public class GetBlockchainQuery : IRequest<GetBlockchainResponse>
{
    public BlockchainIdentifier BlockchainIdentifier { get; set; }
}