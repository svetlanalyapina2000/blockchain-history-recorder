using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;

public class GetBlockchainHistoryMapper : Profile
{
    private static readonly IReadOnlyDictionary<BlockchainIdentifier, string> TypeToString =
        new Dictionary<BlockchainIdentifier, string>
        {
            { BlockchainIdentifier.BitcoinMainnet, "BTC.main" },
            { BlockchainIdentifier.EthereumMainnet, "ETH.main" },
            { BlockchainIdentifier.DashMainnet, "DASH.main" },
            { BlockchainIdentifier.LitecoinMainnet, "LTC.main" },
            { BlockchainIdentifier.BitcoinTestnet, "BTC.test3" }
        };

    public GetBlockchainHistoryMapper()
    {
        CreateMap<Blockchain, GetBlockchainHistoryResponse.BlockchainResponse>();
    }

    public static string? GetStringFromEnum(BlockchainIdentifier identifier)
    {
        return TypeToString.GetValueOrDefault(identifier);
    }
}