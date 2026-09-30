using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Commands.CreateMember;

public sealed record CreateMemberCommand(
    string FullName,
    string? MembershipNumber,
    string? Email,
    string? Phone,
    string? Address,
    DateOnly JoinedDate,
    Guid HomeBranchId,
    byte[]? Photo)
    : IRequest<Result<Guid, Error>>;



