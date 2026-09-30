using FluentValidation;

namespace TitanFitness.Application.Trainers.Contracts;

public sealed record UpdateTrainerRequest(
    string TrainerName,
    Guid BranchId,
    string? Email,
    string? Phone,
    bool IsActive);






public sealed class UpdateTrainerRequestValidator
    : AbstractValidator<UpdateTrainerRequest>
{
    public UpdateTrainerRequestValidator()
    {
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