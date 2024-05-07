using AutoMapper;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Models;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;

public class StoreBlockchainMapper
{
    public class StoreMappingProfile : Profile
    {
        public StoreMappingProfile()
        {
            CreateMap<BlockchainData, Blockchain>();
            CreateMap<Blockchain, StoreBlockchainResponse>();
        }
    }
}