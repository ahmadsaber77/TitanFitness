using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Members.Queries.GetMemberGuestPassSummary;

public sealed class GetMemberGuestPassSummaryQueryHandler
    : IRequestHandler<
        GetMemberGuestPassSummaryQuery,
        Result<MemberGuestPassSummaryResponse, Error>>
{
    private readonly IReadRepository<Membership> _membershipRepository;

    public GetMemberGuestPassSummaryQueryHandler(
        IReadRepository<Membership> membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public async Task<Result<MemberGuestPassSummaryResponse, Error>> Handle(
        GetMemberGuestPassSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.FindAsync(
            x => x.MemberId == query.MemberId,
            cancellationToken);

        if (membership is null)
        {
            return Result.Failure<MemberGuestPassSummaryResponse, Error>(
                Error.NotFound<Membership>(query.MemberId));
        }

        var usedGuestPassCount = membership.GuestPasses.Count;

        var allowedGuestPassCount =
            membership.AgreedTerms.GuestPassQuota;

        var remainingGuestPassCount =
            allowedGuestPassCount - usedGuestPassCount;

        var response = new MemberGuestPassSummaryResponse(
            usedGuestPassCount,
            allowedGuestPassCount,
            remainingGuestPassCount);

        return Result.Success<MemberGuestPassSummaryResponse, Error>(
            response);
    }
}