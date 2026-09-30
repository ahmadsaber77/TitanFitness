using CSharpFunctionalExtensions;
using MediatR;
using System.Security.Cryptography;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Application.Members.Commands.CreateMember;

public sealed class CreateMemberCommandHandler
    : IRequestHandler<
        CreateMemberCommand,
        Result<Guid, Error>>
{
    private readonly IRepository<Member> _memberRepository;
    private readonly IReadRepository<Member> _memberReadRepository;
    private readonly IReadRepository<Branch> _branchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMemberCommandHandler(
        IRepository<Member> memberRepository,
        IReadRepository<Member> memberReadRepository,
        IReadRepository<Branch> branchRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _memberReadRepository = memberReadRepository;
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        CreateMemberCommand command,
        CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdAsync(
            command.HomeBranchId,
            cancellationToken);

        if (branch is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Branch>(command.HomeBranchId));
        }

        MembershipNumber membershipNumber;

        if (string.IsNullOrWhiteSpace(command.MembershipNumber))
        {
            membershipNumber =
                await GenerateMembershipNumberAsync(
                    cancellationToken);
        }
        else
        {
            var membershipNumberResult =
                MembershipNumber.Create(
                    command.MembershipNumber);

            if (membershipNumberResult.IsFailure)
            {
                return Result.Failure<Guid, Error>(
                    membershipNumberResult.Error);
            }

            membershipNumber = membershipNumberResult.Value;
        }

        var existingMember =
            await _memberReadRepository.FindAsync(
                x => x.MembershipNumber.Value ==
                     membershipNumber.Value,
                cancellationToken);

        if (existingMember is not null)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<Member>(
                    $"Membership number '{membershipNumber.Value}' already exists."));
        }

        Email? email = null;

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var emailResult = Email.Create(command.Email);

            if (emailResult.IsFailure)
            {
                return Result.Failure<Guid, Error>(
                    emailResult.Error);
            }

            email = emailResult.Value;
        }

        var memberResult = Member.Create(
            membershipNumber,
            command.FullName,
            command.JoinedDate,
            command.HomeBranchId,
            email,
            command.Phone,
            command.Address,
            command.Photo);

        if (memberResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                memberResult.Error);
        }

        var member = memberResult.Value;

        _memberRepository.Add(member);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success<Guid, Error>(
            member.Id);
    }

    private async Task<MembershipNumber>
        GenerateMembershipNumberAsync(
            CancellationToken cancellationToken)
    {
        while (true)
        {
            var number =
                $"TF-{RandomNumberGenerator.GetInt32(1000, 10000)}";

            var existingMember =
                await _memberReadRepository.FindAsync(
                    x => x.MembershipNumber.Value == number,
                    cancellationToken);

            if (existingMember is null)
            {
                var result = MembershipNumber.Create(number);

                if (result.IsSuccess)
                {
                    return result.Value;
                }
            }
        }
    }
}