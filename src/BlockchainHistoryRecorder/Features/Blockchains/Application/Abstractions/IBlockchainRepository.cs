using BlockchainHistoryRecorder.Domain;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;

public interface IBlockchainRepository : IRepository<Blockchain>;