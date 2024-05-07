using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Infrastructure.Constants;

namespace BlockchainHistoryRecorder.Infrastructure.Persistence.Mappings;

public class DatabaseMapping : Cassandra.Mapping.Mappings
{
    public DatabaseMapping()
    {
        For<Blockchain>()
            .TableName(DbSchemeConstants.BlockchainTable)
            .PartitionKey(u => u.Name)
            .ClusteringKey(u => u.CreatedAt);
    }
}