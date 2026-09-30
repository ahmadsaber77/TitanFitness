using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Queries.GetMemberFreezeSummary;

public sealed record GetMemberFreezeSummaryQuery(Guid MemberId)
    : IRequest<Result<MemberFreezeSummaryResponse, Error>>;
