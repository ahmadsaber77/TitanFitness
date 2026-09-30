using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Plans.Commands.CreatePlan;

public sealed class CreatePlanCommandHandler
    : IRequestHandler<CreatePlanCommand, Result<Guid, Error>>
{
    private readonly IRepository<Plan> _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlanCommandHandler(
        IRepository<Plan> planRepository,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        CreatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var planResult = Plan.Create(
            command.PlanName,
            command.Price,
            command.DurationInMonths,
            command.MaxFreezeDays,
            command.MaxNumberOfFreezes,
            command.GuestPassQuota,
            command.AccessScope,
            command.IsPublished);

        if (planResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                planResult.Error);
        }

        var plan = planResult.Value;

        _planRepository.Add(plan);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success<Guid, Error>(
            plan.Id);
    }
}