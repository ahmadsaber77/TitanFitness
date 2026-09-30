using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.ClassSessions.Commands.CreateClassSession;

public sealed record CreateClassSessionCommand(
    string ClassName,
    Guid BranchId,
    Guid StudioId,
    Guid TrainerId,
    DateOnly SessionDate,
    TimeOnly StartTime,
    int DurationMinutes,
    int CapacityLimit,
    string? Description)
    : IRequest<Result<Guid, Error>>;





