using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Queries.GetMemberMembership;

public sealed record GetMemberMembershipQuery(Guid MemberId)
    : IRequest<Result<MemberMembershipResponse, Error>>;
