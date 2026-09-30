namespace TitanFitness.Application.Members.Contracts;

public sealed record MemberFreezeSummaryResponse(
    int UsedFreezeCount,
    int AllowedFreezeCount,
    int RemainingFreezeCount);
