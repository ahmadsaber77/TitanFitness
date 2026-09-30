using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.ClassSessions.Contracts;

public sealed record TodayClassSessionResponse(
    Guid Id,
    TimeOnly StartTime,
    string ClassName,
    string TrainerName,
    string StudioName,
    int BookedCount,
    int CapacityLimit,
    int WaitlistCount,
    ClassSessionStatus Status);





