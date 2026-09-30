using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Queries.GetMemberActivities;

public sealed record GetMemberActivitiesQuery(
Guid MemberId)
: IRequest<Result<MemberActivitiesResponse, Error>>;
