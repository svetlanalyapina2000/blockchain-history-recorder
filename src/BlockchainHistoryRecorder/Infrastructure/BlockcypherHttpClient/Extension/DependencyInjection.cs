using BlockchainHistoryRecorder.Application.Abstraction;

namespace BlockchainHistoryRecorder.Infrastructure.BlockcypherHttpClient.Extension;

public static class DependencyInjection
{
    public static void AddBlockcypherHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IBlockcypherHttpClient, BlockcypherHttpClient>((_, client) =>
            {
                client.BaseAddress = new Uri("https://api.blockcypher.com");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
    }
}