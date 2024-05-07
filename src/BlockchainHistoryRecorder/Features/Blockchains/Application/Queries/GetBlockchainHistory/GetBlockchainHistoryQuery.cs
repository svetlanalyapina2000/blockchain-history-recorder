using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using MediatR;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;

public class GetBlockchainHistoryQuery : IRequest<GetBlockchainHistoryResponse>
{
    public BlockchainIdentifier BlockchainIdentifier { get; set; }
}