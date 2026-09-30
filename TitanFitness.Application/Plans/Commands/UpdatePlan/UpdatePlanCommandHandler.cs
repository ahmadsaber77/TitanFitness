using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Plans.Commands.UpdatePlan;

public sealed class UpdatePlanCommandHandler
    : IRequestHandler<UpdatePlanCommand, Result<Guid, Error>>
{
    private readonly IRepository<Plan> _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePlanCommandHandler(
        IRepository<Plan> planRepository,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        UpdatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(
            command.PlanId,
            cancellationToken);

        if (plan is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Plan>(command.PlanId));
        }

        var updateResult = plan.Update(
            command.PlanName,
            command.Price,
            command.DurationInMonths,
            command.MaxFreezeDays,
            command.MaxNumberOfFreezes,
            command.GuestPassQuota,
            command.AccessScope,
            command.IsPublished);

        if (updateResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success<Guid, Error>(
            plan.Id);
    }
}