using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Queries.GetMemberGuestPassSummary;

public sealed record GetMemberGuestPassSummaryQuery(Guid MemberId)
    : IRequest<Result<MemberGuestPassSummaryResponse, Error>>;