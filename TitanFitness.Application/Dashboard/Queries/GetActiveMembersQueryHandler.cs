using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Application.Dashboard.Contracts.Responses;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Dashboard.Queries;

public class GetActiveMembersQueryHandler : IRequestHandler<
        GetActiveMembersQuery,
        Result<ActiveMembersResponse, Error>>
{

    private readonly IReadRepository<Membership> _membershipRepository;
    private readonly IReadRepository<CheckIn> _checkInRepository;

    public GetActiveMembersQueryHandler(
        IReadRepository<Membership> membershipRepository,
        IReadRepository<CheckIn> checkInRepository)
    {
        _membershipRepository = membershipRepository;
        _checkInRepository = checkInRepository;
    }


    public async Task<Result<ActiveMembersResponse, Error>> Handle(
    GetActiveMembersQuery query,
    CancellationToken cancellationToken)
    {
        var memberships = _membershipRepository.Query();

        var activeCount = await memberships
            .CountAsync(
                x => x.Status == MembershipStatus.Active,
                cancellationToken);

        var checkIns = _checkInRepository.Query();

        var insideCount = await checkIns
            .Where(x =>
                x.Result == CheckInResult.Admitted &&
                x.CheckOutDateTime == null)
            .Select(x => x.MemberId)
            .Distinct()
            .CountAsync(cancellationToken);

        var response = new ActiveMembersResponse(
            activeCount,
            insideCount);

        return Result.Success<ActiveMembersResponse, Error>(response);
    }
}
