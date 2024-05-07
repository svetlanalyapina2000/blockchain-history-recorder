using Cassandra;

namespace BlockchainHistoryRecorder.Infrastructure.Persistence.Extension;

public static class DependencyInjection
{
    public static void AddCassandra(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetValue<string>("CASSANDRA_HOST");
        var port = configuration.GetValue("CASSANDRA_PORT", 9042);
        var keyspace = configuration.GetValue<string>("CASSANDRA_KEYSPACE");

        var cluster = Cluster.Builder()
            .AddContactPoint(host)
            .WithPort(port)
            .WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalQuorum))
            .WithRetryPolicy(new LoggingRetryPolicy(new DefaultRetryPolicy()))
            .Build();

        var session = cluster.Connect(keyspace);
        services.AddSingleton(session);
    }
}