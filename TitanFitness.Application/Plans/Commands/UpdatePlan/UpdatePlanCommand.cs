using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Plans.Commands.UpdatePlan;

public sealed record UpdatePlanCommand(
    Guid PlanId,
    string PlanName,
    decimal Price,
    int DurationInMonths,
    bool IsPublished,
    int MaxFreezeDays,
    int MaxNumberOfFreezes,
    int GuestPassQuota,
    AccessScope AccessScope)
    : IRequest<Result<Guid, Error>>;