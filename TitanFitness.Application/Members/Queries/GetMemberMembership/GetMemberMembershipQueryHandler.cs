using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Members.Queries.GetMemberMembership;

public sealed class GetMemberMembershipQueryHandler
    : IRequestHandler<
        GetMemberMembershipQuery,
        Result<MemberMembershipResponse, Error>>
{
    private readonly IReadRepository<Membership> _membershipRepository;
    private readonly IReadRepository<Plan> _planRepository;

    public GetMemberMembershipQueryHandler(
        IReadRepository<Membership> membershipRepository,
        IReadRepository<Plan> planRepository)
    {
        _membershipRepository = membershipRepository;
        _planRepository = planRepository;
    }

    public async Task<Result<MemberMembershipResponse, Error>> Handle(
        GetMemberMembershipQuery query,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.FindAsync(
            x => x.MemberId == query.MemberId,
            cancellationToken);

        if (membership is null)
        {
            return Result.Failure<MemberMembershipResponse, Error>(
                Error.NotFound<Membership>(query.MemberId));
        }

        var plan = await _planRepository.GetByIdAsync(
            membership.PlanId,
            cancellationToken);

        if (plan is null)
        {
            return Result.Failure<MemberMembershipResponse, Error>(
                Error.NotFound<Plan>(membership.PlanId));
        }

        var response = new MemberMembershipResponse(
            membership.Id,
            plan.Name,
            membership.AgreedTerms.Price,
            membership.AgreedTerms.DurationInMonths,
            membership.StartDate,
            membership.EndDate,
            membership.Status,
            membership.AgreedTerms.MaxFreezeDays,
            membership.AgreedTerms.MaxNumberOfFreezes,
            membership.AgreedTerms.GuestPassQuota,
            membership.AgreedTerms.AccessScope);

        return Result.Success<MemberMembershipResponse, Error>(
            response);
    }
}
