using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Features.Blockchains.Infrastructure.BlockchainHttpClient;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;

public class GetBlockchainMapper : Profile
{
    private static readonly IReadOnlyDictionary<BlockchainIdentifier, string> TypeToString =
        new Dictionary<BlockchainIdentifier, string>
        {
            { BlockchainIdentifier.BitcoinMainnet, "btc/main" },
            { BlockchainIdentifier.EthereumMainnet, "eth/main" },
            { BlockchainIdentifier.DashMainnet, "dash/main" },
            { BlockchainIdentifier.LitecoinMainnet, "ltc/main" },
            { BlockchainIdentifier.BitcoinTestnet, "btc/test3" }
        };

    public GetBlockchainMapper()
    {
        CreateMap<BlockchainDTO, BlockchainData>();
    }

    public static string? GetStringFromEnum(BlockchainIdentifier identifier)
    {
        return TypeToString.GetValueOrDefault(identifier);
    }
}