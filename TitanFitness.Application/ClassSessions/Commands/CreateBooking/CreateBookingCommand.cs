using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.ClassSessions.Commands.CreateBooking;

public sealed record CreateBookingCommand(
    Guid SessionId,
    Guid MemberId,
    string? TrainerNotes)
    : IRequest<Result<Guid, Error>>;
