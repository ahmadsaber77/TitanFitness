namespace TitanFitness.Application.Dashboard.Contracts.Responses;

public sealed record TodayCheckInsResponse(
    int count ,
    int LastWeekCount);