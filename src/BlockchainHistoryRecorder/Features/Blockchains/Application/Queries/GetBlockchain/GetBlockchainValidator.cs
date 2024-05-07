using FluentValidation;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;

public class GetBlockchainValidator : AbstractValidator<GetBlockchainQuery>
{
    public GetBlockchainValidator()
    {
        RuleFor(m => m.BlockchainIdentifier)
            .IsInEnum().WithMessage("Invalid blockchain type provided.");
    }
}