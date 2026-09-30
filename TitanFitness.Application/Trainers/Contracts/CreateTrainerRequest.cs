using FluentValidation;
using TitanFitness.Application.Trainers.Commands.CreateTrainer;

namespace TitanFitness.Application.Trainers.Contracts;

public sealed record CreateTrainerRequest(
    string TrainerNumber,
    string TrainerName,
    Guid BranchId,
    string? Email,
    string? Phone,
    bool IsActive);




public sealed class CreateTrainerCommandValidator
    : AbstractValidator<CreateTrainerCommand>
{
    public CreateTrainerCommandValidator()
    {
        RuleFor(x => x.TrainerNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.TrainerName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BranchId)
            .NotEmpty();

        RuleFor(x => x.Email)
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .MaximumLength(20);
    }
}