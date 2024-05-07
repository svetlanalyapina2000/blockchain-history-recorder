using BlockchainHistoryRecorder.Features.Blockchains.Domain;

namespace BlockchainHistoryRecorder.Tests.Integration.Features.Blockchain.TestData;

public static class BlockchainIdentifierTestData
{
    public static IEnumerable<object[]> BlockchainIdentifiers => GetBlockchainIdentifiers();

    private static IEnumerable<object[]> GetBlockchainIdentifiers()
    {
        return Enum.GetValues(typeof(BlockchainIdentifier))
            .Cast<BlockchainIdentifier>()
            .Where(bi => bi != BlockchainIdentifier.Unknown)
            .Select(bi => new object[] { bi });
    }
}