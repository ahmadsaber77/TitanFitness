using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Freezes.Commands.FreezeMembership;

public sealed record FreezeMembershipCommand(
    Guid MemberId,
    DateOnly StartDate,
    int DurationInMonths,
    string Reason,
    string? AdditionalNotes)
    : IRequest<Result<Guid, Error>>;
