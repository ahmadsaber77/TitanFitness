using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Commands.UpdateMember;

public sealed record UpdateMemberCommand (
    Guid MemberId,
    string FullName,
    string? Email,
    string? Phone,
    string? Address,
    DateOnly JoinedDate,
    Guid HomeBranchId,
    byte[]? Photo)
    : IRequest<Result<Guid, Error>>;
