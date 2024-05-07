using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;
using MediatR;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;

public class StoreBlockchainCommand : IRequest<StoreBlockchainResponse>
{
    public required BlockchainData Blockchain { get; set; }
}