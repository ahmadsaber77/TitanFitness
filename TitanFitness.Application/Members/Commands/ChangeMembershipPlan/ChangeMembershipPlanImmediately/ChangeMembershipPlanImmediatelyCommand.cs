using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Commands.ChangeMembershipPlan.ChangeMembershipPlanImmediately;

public sealed record ChangeMembershipPlanImmediatelyCommand(
    Guid MemberId,
    Guid PlanId)
    : IRequest<Result<Guid, Error>>;