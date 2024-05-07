using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;
using FluentValidation;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;

public class GetBlockchainHistoryValidator : AbstractValidator<GetBlockchainQuery>
{
    public GetBlockchainHistoryValidator()
    {
        RuleFor(m => m.BlockchainIdentifier)
            .IsInEnum().WithMessage("Invalid blockchain type provided.");
    }
}