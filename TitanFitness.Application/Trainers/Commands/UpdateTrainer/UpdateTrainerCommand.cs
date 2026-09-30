using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Trainers.Commands.UpdateTrainer;

public sealed record UpdateTrainerCommand(
    Guid TrainerId,
    string TrainerName,
    Guid BranchId,
    string? Email,
    string? Phone,
    bool IsActive)
    : IRequest<Result<Guid, Error>>;
