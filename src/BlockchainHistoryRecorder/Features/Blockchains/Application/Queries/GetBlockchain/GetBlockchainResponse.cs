using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;

public record GetBlockchainResponse
{
    public required BlockchainData Blockchain { get; set; }
}