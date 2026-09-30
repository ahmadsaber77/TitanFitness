namespace TitanFitness.Application.Dashboard.Contracts.Responses;

public sealed record UpcomingClassResponse(
    Guid SessionId,
    DateOnly SessionDate,
    TimeOnly StartTime,
    string StudioName,
    string TrainerName,
    int PlacesTaken,
    int Capacity,
    string Status);
