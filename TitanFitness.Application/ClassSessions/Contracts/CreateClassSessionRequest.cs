using FluentValidation;
using TitanFitness.Application.ClassSessions.Commands.CreateClassSession;

namespace TitanFitness.Application.ClassSessions.Contracts;

public sealed record CreateClassSessionRequest(
    string ClassName,
    Guid BranchId,
    Guid StudioId,
    Guid TrainerId,
    DateOnly SessionDate,
    TimeOnly StartTime,
    int DurationMinutes,
    int CapacityLimit,
    string? Description);



public sealed class CreateClassSessionCommandValidator
    : AbstractValidator<CreateClassSessionRequest>
{
    public CreateClassSessionCommandValidator()
    {
        RuleFor(x => x.ClassName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BranchId)
            .NotEmpty();

        RuleFor(x => x.StudioId)
            .NotEmpty();

        RuleFor(x => x.TrainerId)
            .NotEmpty();

        RuleFor(x => x.DurationMinutes)
            .Must(x => x is 30 or 45 or 60)
            .WithMessage("Duration must be 30, 45, or 60 minutes.");

        RuleFor(x => x.CapacityLimit)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
