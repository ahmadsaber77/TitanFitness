using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Plans.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Plans.Queries.GetPlanDetails;

public sealed class GetPlanDetailsQueryHandler
    : IRequestHandler<
        GetPlanDetailsQuery,
        Result<PlanDetailsResponse, Error>>
{
    private readonly IReadRepository<Plan> _planRepository;

    public GetPlanDetailsQueryHandler(
        IReadRepository<Plan> planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<Result<PlanDetailsResponse, Error>> Handle(
        GetPlanDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(
            query.PlanId,
            cancellationToken);

        if (plan is null)
        {
            return Result.Failure<PlanDetailsResponse, Error>(
                Error.NotFound<Plan>(query.PlanId));
        }

        var response = new PlanDetailsResponse(
            plan.Id,
            plan.Name,
            plan.Price,
            plan.DurationInMonths,
            plan.MaxFreezeDays,
            plan.MaxNumberOfFreezes,
            plan.GuestPassQuota,
            plan.AccessScope,
            plan.IsPublished);

        return Result.Success<PlanDetailsResponse, Error>(
            response);
    }
}