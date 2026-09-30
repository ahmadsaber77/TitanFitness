namespace TitanFitness.Application.Dashboard.Contracts.Responses;

public sealed record ActiveMembersResponse(
    int ActiveCount,
    int InsideCount);

