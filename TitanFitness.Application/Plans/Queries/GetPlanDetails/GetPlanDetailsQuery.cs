using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Plans.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Plans.Queries.GetPlanDetails;

public sealed record GetPlanDetailsQuery(
    Guid PlanId)
    : IRequest<Result<PlanDetailsResponse, Error>>;