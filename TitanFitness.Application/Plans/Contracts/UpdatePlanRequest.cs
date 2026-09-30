using FluentValidation;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Plans.Contracts;

public sealed record UpdatePlanRequest(
    string PlanName,
    decimal Price,
    int DurationInMonths,
    bool IsPublished,
    int MaxFreezeDays,
    int MaxNumberOfFreezes,
    int GuestPassQuota,
    AccessScope AccessScope);





public sealed class UpdatePlanRequestValidator
    : AbstractValidator<UpdatePlanRequest>
{
    public UpdatePlanRequestValidator()
    {
        RuleFor(x => x.PlanName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DurationInMonths)
            .GreaterThan(0);

        RuleFor(x => x.MaxFreezeDays)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MaxNumberOfFreezes)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.GuestPassQuota)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.AccessScope)
            .IsInEnum();
    }
}
