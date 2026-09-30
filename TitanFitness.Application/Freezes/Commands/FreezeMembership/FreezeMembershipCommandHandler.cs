using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Freezes.Commands.FreezeMembership;

public sealed class FreezeMembershipCommandHandler
    : IRequestHandler<
        FreezeMembershipCommand,
        Result<Guid, Error>>
{
    private readonly IRepository<Membership> _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FreezeMembershipCommandHandler(
        IRepository<Membership> membershipRepository,
        IUnitOfWork unitOfWork)
    {
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        FreezeMembershipCommand command,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.FindAsync(
            x => x.MemberId == command.MemberId,
            cancellationToken);

        if (membership is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Membership>(command.MemberId));
        }

        var result = membership.RequestFreeze(
            command.StartDate,
            command.DurationInMonths,
            command.Reason,
            command.AdditionalNotes);

        if (result.IsFailure)
        {
            return Result.Failure<Guid, Error>(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid, Error>(
            result.Value.Id);
    }
}
