using Azure;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Application.Common;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TitanFitness.Application.Members.Queries.GetMembers;

public class GetMembersQueryHandler : IRequestHandler<
        GetMembersQuery,
        Result<MembersResponse, Error>>
{
    private readonly IReadRepository<Member> _memberRepository;
    private readonly IReadRepository<Membership> _membershipRepository;
    private readonly IReadRepository<Branch> _branchRepository;
    private readonly IReadRepository<CheckIn> _checkInRepository;

    public GetMembersQueryHandler(
        IReadRepository<Member> memberRepository,
        IReadRepository<Membership> membershipRepository,
        IReadRepository<Branch> branchRepository,
        IReadRepository<CheckIn> checkInRepository)
    {
        _memberRepository = memberRepository;
        _membershipRepository = membershipRepository;
        _branchRepository = branchRepository;
        _checkInRepository = checkInRepository;
    }

   
       public async Task<Result<MembersResponse, Error>> Handle(
        GetMembersQuery query,
        CancellationToken cancellationToken)
    {

        var membersQuery =
            from branch in _branchRepository.Query()
            join member in _memberRepository.Query()
            on branch.Id equals member.HomeBranchId

            let membership =
                _membershipRepository.Query()
                    .Where(x => x.MemberId == member.Id)
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault()

            let lastVisit =
                _checkInRepository.Query()
                    .Where(x =>
                        x.MemberId == member.Id &&
                        x.Result == CheckInResult.Admitted)
                    .OrderByDescending(x => x.CheckInDateTime)
                    .Select(x => (DateTime?)x.CheckInDateTime)
                    .FirstOrDefault()

            select new
            {
                Member = member,
                Branch = branch,
                Membership = membership,
                LastVisit = lastVisit
            };

        if (query.BranchId.HasValue)
        {
            membersQuery = membersQuery.Where(x =>
                x.Member.HomeBranchId == query.BranchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            var matchingStatuses = Enum
                .GetValues<MembershipStatus>()
                .Where(status =>
                    status.ToString()
                        .Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                .ToList();

            membersQuery = membersQuery.Where(x =>
                x.Member.FullName.Contains(search) ||
                x.Member.MembershipNumber.Value.Contains(search) ||
                x.Branch.Name.Contains(search) ||
                x.Membership != null &&
                 matchingStatuses.Contains(x.Membership.Status));
        }

        var totalCount = await membersQuery.CountAsync(
            cancellationToken);

        var members = await membersQuery
            .OrderBy(x => x.Member.FullName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = members
            .Select(x => new MemberDirectoryResponse(
                x.Member.Id,
                x.Member.FullName,
                x.Member.MembershipNumber.Value,
                x.Membership?.Status ?? MembershipStatus.Expired,
                x.Branch.Name,
                x.LastVisit))
            .ToList();
        var pagenation = new PaginationResponse(query.Page,
            query.PageSize,
            totalCount);

        var response = new MembersResponse(
            items,
           pagenation);

        return Result.Success<MembersResponse, Error>(response);
    }
}
