namespace TitanFitness.Application.Members.Contracts;

public sealed record MemberGuestPassSummaryResponse(
    int UsedGuestPassCount,
    int AllowedGuestPassCount,
    int RemainingGuestPassCount);