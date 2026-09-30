using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Commands.ChangeMembershipPlan.ChangeMembershipPlanAtRenewal;

public sealed record ChangeMembershipPlanAtRenewalCommand(
    Guid MemberId,
    Guid PlanId)
    : IRequest<Result<Guid, Error>>;
