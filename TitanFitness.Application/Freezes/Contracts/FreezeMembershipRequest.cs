using FluentValidation;
using TitanFitness.Application.Freezes.Commands.FreezeMembership;

namespace TitanFitness.Application.Freezes.Contracts;

public sealed record FreezeMembershipRequest(
    DateOnly StartDate,
    int DurationInMonths,
    string Reason,
    string? AdditionalNotes);


public sealed class FreezeMembershipCommandValidator
    : AbstractValidator<FreezeMembershipCommand>
{
    public FreezeMembershipCommandValidator()
    {
        RuleFor(x => x.DurationInMonths)
            .InclusiveBetween(1, 3);

        RuleFor(x => x.Reason)
            .NotEmpty();

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(200);
    }
}