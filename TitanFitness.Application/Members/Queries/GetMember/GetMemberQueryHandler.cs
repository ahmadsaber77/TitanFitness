using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Application.Members.Queries.GetMembers;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Members.Queries.GetMemberProfile;

public sealed class GetMemberQueryHandler
    : IRequestHandler<
        GetMemberQuery,
        Result<MemberResponse, Error>>
{
    private readonly IReadRepository<Member> _memberRepository;

    public GetMemberQueryHandler(
        IReadRepository<Member> memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<Result<MemberResponse, Error>> Handle(
        GetMemberQuery query,
        CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(
            query.MemberId,
            cancellationToken);

        if (member is null)
        {
            return Result.Failure<MemberResponse, Error>(
                Error.NotFound<Member>(query.MemberId));
        }

        var response = new MemberResponse(
            member.Id,
            member.FullName,
            member.MembershipNumber.Value,
            member.Email?.Value,
            member.Phone?.Value,
            member.Address,
            member.JoinedDate,
            member.Photo);

        return Result.Success<MemberResponse, Error>(
            response);
    }
}
