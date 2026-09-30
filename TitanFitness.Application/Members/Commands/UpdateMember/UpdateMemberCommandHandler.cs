using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Application.Members.Commands.UpdateMember;

sealed class UpdateMemberCommandHandler
    : IRequestHandler<
        UpdateMemberCommand,
        Result<Guid, Error>>
{
    private readonly IReadRepository<Branch> _branchRepository;
    private readonly IRepository<Member> _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMemberCommandHandler(
        IReadRepository<Branch> branchRepository,
        IRepository<Member> memberRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        UpdateMemberCommand command,
        CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(
           command.MemberId,
           cancellationToken);

        if (member is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Member>(command.MemberId));
        }

        var branch = await _branchRepository.GetByIdAsync(
            command.HomeBranchId,
            cancellationToken);

        if (branch is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Branch>(command.HomeBranchId));
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

        var updateResult = member.UpdateProfile(
            command.FullName,
            command.JoinedDate,
            command.HomeBranchId,
            email,
            command.Phone,
            command.Address,
            command.Photo);

        if (updateResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success<Guid, Error>(
            member.Id);
    }
}
    



