using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Members.Queries.GetMemberFreezeSummary;

public sealed class GetMemberFreezeSummaryQueryHandler
    : IRequestHandler<
        GetMemberFreezeSummaryQuery,
        Result<MemberFreezeSummaryResponse, Error>>
{
    private readonly IReadRepository<Membership> _membershipRepository;

    public GetMemberFreezeSummaryQueryHandler(
        IReadRepository<Membership> membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public async Task<Result<MemberFreezeSummaryResponse, Error>> Handle(
        GetMemberFreezeSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.FindAsync(
            x => x.MemberId == query.MemberId,
            cancellationToken);

        if (membership is null)
        {
            return Result.Failure<MemberFreezeSummaryResponse, Error>(
                Error.NotFound<Membership>(query.MemberId));
        }

        var usedFreezeCount = membership.Freezes.Count;

        var allowedFreezeCount =
            membership.AgreedTerms.MaxNumberOfFreezes;

        var remainingFreezeCount =
            allowedFreezeCount - usedFreezeCount;

        var response = new MemberFreezeSummaryResponse(
            usedFreezeCount,
            allowedFreezeCount,
            remainingFreezeCount);

        return Result.Success<MemberFreezeSummaryResponse, Error>(
            response);
    }
}