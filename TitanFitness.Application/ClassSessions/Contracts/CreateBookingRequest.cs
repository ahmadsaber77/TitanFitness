using FluentValidation;
using TitanFitness.Application.ClassSessions.Commands.CreateBooking;

namespace TitanFitness.Application.ClassSessions.Contracts;

public sealed record CreateBookingRequest(
    Guid MemberId,
    string? TrainerNotes);



public sealed class CreateBookingRequestValidator
    : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty();

        RuleFor(x => x.TrainerNotes)
            .MaximumLength(500);
    }
}

