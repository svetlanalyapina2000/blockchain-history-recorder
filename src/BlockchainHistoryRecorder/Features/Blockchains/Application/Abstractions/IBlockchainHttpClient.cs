using BlockchainHistoryRecorder.Features.Blockchains.Infrastructure.BlockchainHttpClient;

// ReSharper disable once CheckNamespace
namespace BlockchainHistoryRecorder.Application.Abstraction;

public partial interface IBlockcypherHttpClient
{
    Task<BlockchainDTO?> GetLatestBlockchainAsync(string name, CancellationToken cancellationToken);
}