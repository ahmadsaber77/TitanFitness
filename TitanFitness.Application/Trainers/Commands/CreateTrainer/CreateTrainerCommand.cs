using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Trainers.Commands.CreateTrainer;

public sealed record CreateTrainerCommand(
    string TrainerNumber,
    string TrainerName,
    Guid BranchId,
    string? Email,
    string? Phone,
    bool IsActive)
    : IRequest<Result<Guid, Error>>;


