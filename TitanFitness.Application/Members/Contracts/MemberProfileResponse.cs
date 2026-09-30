using FluentValidation;
using TitanFitness.Application.Members.Queries.GetMemberProfile;

namespace TitanFitness.Application.Members.Contracts;

public sealed record MemberResponse(
    Guid Id,
    string FullName,
    string MembershipNumber,
    string? Email,
    string? Phone,
    string? Address,
    DateOnly JoinedDate,
    byte[]? Photo);



