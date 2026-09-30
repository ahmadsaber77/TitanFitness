using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Members.Commands.ChangeMembershipPlan.ChangeMembershipPlanAtRenewal;

public sealed class ChangeMembershipPlanAtRenewalCommandHandler
    : IRequestHandler<
        ChangeMembershipPlanAtRenewalCommand,
        Result<Guid, Error>>
{
    private readonly IReadRepository<Membership> _membershipReadRepository;
    private readonly IRepository<Membership> _membershipRepository;
    private readonly IReadRepository<Plan> _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeMembershipPlanAtRenewalCommandHandler(
     IReadRepository<Membership> membershipReadRepository,
     IRepository<Membership> membershipRepository,
     IReadRepository<Plan> planRepository,
     IUnitOfWork unitOfWork)
    {
        _membershipReadRepository = membershipReadRepository;
        _membershipRepository = membershipRepository;
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        ChangeMembershipPlanAtRenewalCommand command,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipReadRepository.FindAsync(
          x => x.MemberId == command.MemberId,
          cancellationToken);

        if (membership is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Membership>(command.MemberId));
        }

        var plan = await _planRepository.GetByIdAsync(
            command.PlanId,
            cancellationToken);

        if (plan is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Plan>(command.PlanId));
        }

        var result = membership.ChangePlanAtRenewal(plan);

        if (result.IsFailure)
        {
            return Result.Failure<Guid, Error>(result.Error);
        }

        var newMembership = result.Value;

        _membershipRepository.Add(newMembership);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid, Error>(newMembership.Id);
    }
}
