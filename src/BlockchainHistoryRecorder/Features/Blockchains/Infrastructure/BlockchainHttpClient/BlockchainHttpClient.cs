using System.Text.Json;
using BlockchainHistoryRecorder.Features.Blockchains.Infrastructure.BlockchainHttpClient;

// ReSharper disable once CheckNamespace
namespace BlockchainHistoryRecorder.Infrastructure.BlockcypherHttpClient;

public partial class BlockcypherHttpClient
{
    public async Task<BlockchainDTO?> GetLatestBlockchainAsync(string name, CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };

        _logger.LogInformation("Starting to fetch the latest blockchain data for {BlockchainName}", name);

        return await _client.GetFromJsonAsync<BlockchainDTO>($"v1/{name}", options, cancellationToken)
               ?? throw new InvalidOperationException();
    }
}