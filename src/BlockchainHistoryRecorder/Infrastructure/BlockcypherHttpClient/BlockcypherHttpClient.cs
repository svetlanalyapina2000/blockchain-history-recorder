using BlockchainHistoryRecorder.Application.Abstraction;

namespace BlockchainHistoryRecorder.Infrastructure.BlockcypherHttpClient;

public partial class BlockcypherHttpClient : IBlockcypherHttpClient
{
    private readonly HttpClient _client;
    private readonly ILogger<BlockcypherHttpClient> _logger;

    public BlockcypherHttpClient(HttpClient client, ILogger<BlockcypherHttpClient> logger)
    {
        _client = client;
        _logger = logger;
    }
}