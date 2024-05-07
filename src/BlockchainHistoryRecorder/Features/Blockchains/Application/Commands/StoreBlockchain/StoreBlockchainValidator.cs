using FluentValidation;

namespace BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;

public class StoreBlockchainValidator : AbstractValidator<StoreBlockchainCommand>
{
    public StoreBlockchainValidator()
    {
        RuleFor(command => command.Blockchain.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(command => command.Blockchain.Height)
            .GreaterThan(0).WithMessage("Height must be greater than zero.");

        RuleFor(command => command.Blockchain.Hash)
            .NotEmpty().WithMessage("Hash is required.");

        RuleFor(command => command.Blockchain.Time)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Time cannot be in the future.");

        RuleFor(command => command.Blockchain.LatestUrl)
            .NotEmpty().WithMessage("Latest URL is required.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Latest URL must be a valid URL.");

        RuleFor(command => command.Blockchain.PreviousHash)
            .NotEmpty().WithMessage("Previous hash is required when available.");

        RuleFor(command => command.Blockchain.PreviousUrl)
            .NotEmpty().WithMessage("Previous URL is required when available.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Previous URL must be a valid URL.");

        RuleFor(command => command.Blockchain.PeerCount)
            .GreaterThanOrEqualTo(0).WithMessage("Peer count cannot be negative.");

        RuleFor(command => command.Blockchain.UnconfirmedCount)
            .GreaterThanOrEqualTo(0).WithMessage("Unconfirmed count cannot be negative.");

        RuleFor(command => command.Blockchain.LastForkHeight)
            .Must(height => height == null || height >= 0)
            .WithMessage("Last fork height cannot be negative if specified.");
    }
}