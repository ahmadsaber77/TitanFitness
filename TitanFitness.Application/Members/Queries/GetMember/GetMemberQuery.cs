using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Queries.GetMemberProfile;

public sealed record GetMemberQuery(
    Guid MemberId)
    : IRequest<Result<MemberResponse, Error>>;

