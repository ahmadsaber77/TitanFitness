namespace TitanFitness.Application.Trainers.Contracts;

public sealed record TrainerDetailsResponse(
    Guid Id,
    string TrainerNumber,
    string TrainerName,
    string BranchName,
    string? Email,
    string? Phone,
    bool IsActive);