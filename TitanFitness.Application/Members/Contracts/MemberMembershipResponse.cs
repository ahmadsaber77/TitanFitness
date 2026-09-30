using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Members.Contracts;

public sealed record MemberMembershipResponse(
    Guid Id,
    string PlanName,
    decimal Price,
    int DurationInMonths,
    DateOnly StartDate,
    DateOnly EndDate,
    MembershipStatus Status,
    int MaxFreezeDays,
    int MaxNumberOfFreezes,
    int GuestPassQuota,
    AccessScope AccessScope);
