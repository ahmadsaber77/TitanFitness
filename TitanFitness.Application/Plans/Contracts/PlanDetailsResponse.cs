using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Plans.Contracts;

public sealed record PlanDetailsResponse(
    Guid Id,
    string Name,
    decimal Price,
    int DurationInMonths,
    int MaxFreezeDays,
    int MaxNumberOfFreezes,
    int GuestPassQuota,
    AccessScope AccessScope,
    bool IsPublished);
