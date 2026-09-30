using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.ClassSessions.Commands.CreateClassSession;

public sealed class CreateClassSessionCommandHandler
    : IRequestHandler<
        CreateClassSessionCommand,
        Result<Guid, Error>>
{
    private readonly IReadRepository<Branch> _branchRepository;
    private readonly IReadRepository<Studio> _studioRepository;
    private readonly IReadRepository<Trainer> _trainerRepository;
    private readonly IReadRepository<ClassSession> _classSessionReadRepository;
    private readonly IRepository<ClassSession> _classSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClassSessionCommandHandler(
        IReadRepository<Branch> branchRepository,
        IReadRepository<Studio> studioRepository,
        IReadRepository<Trainer> trainerRepository,
        IReadRepository<ClassSession> classSessionReadRepository,
        IRepository<ClassSession> classSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _studioRepository = studioRepository;
        _trainerRepository = trainerRepository;
        _classSessionReadRepository = classSessionReadRepository;
        _classSessionRepository = classSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        CreateClassSessionCommand command,
        CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdAsync(
            command.BranchId,
            cancellationToken);

        if (branch is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Branch>(command.BranchId));
        }

        var studio = await _studioRepository.GetByIdAsync(
            command.StudioId,
            cancellationToken);

        if (studio is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Studio>(command.StudioId));
        }

        if (studio.BranchId != command.BranchId)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "The selected studio does not belong to the selected branch."));
        }

        if (command.CapacityLimit > studio.Capacity)
        {
            return Result.Failure<Guid, Error>(
                Error.Validation<ClassSession>(
                    "Capacity limit cannot exceed the studio capacity."));
        }

        var trainer = await _trainerRepository.GetByIdAsync(
            command.TrainerId,
            cancellationToken);

        if (trainer is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Trainer>(command.TrainerId));
        }

        if (!trainer.IsActive)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "The selected trainer is inactive."));
        }

        if (trainer.BranchId != command.BranchId)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "The selected trainer does not belong to the selected branch."));
        }

        var sessionStart = command.StartTime;

        var sessionEnd = command.StartTime.AddMinutes(
            command.DurationMinutes);

        var sameDaySessions = await _classSessionReadRepository
            .Query()
            .Where(x =>
                x.SessionDate == command.SessionDate &&
                x.Status != ClassSessionStatus.Cancelled &&
                (x.TrainerId == command.TrainerId ||
                 x.StudioId == command.StudioId))
            .ToListAsync(cancellationToken);

        var trainerHasOverlap = sameDaySessions.Any(x =>
            x.TrainerId == command.TrainerId &&
            IsOverlapping(
                sessionStart,
                sessionEnd,
                x.StartTime,
                x.StartTime.AddMinutes(x.DurationMinutes)));

        if (trainerHasOverlap)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "The selected trainer already has an overlapping session."));
        }

        var studioHasOverlap = sameDaySessions.Any(x =>
            x.StudioId == command.StudioId &&
            IsOverlapping(
                sessionStart,
                sessionEnd,
                x.StartTime,
                x.StartTime.AddMinutes(x.DurationMinutes)));

        if (studioHasOverlap)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "The selected studio already has an overlapping session."));
        }

        var sessionResult = ClassSession.Create(
            command.ClassName,
            command.BranchId,
            command.StudioId,
            command.TrainerId,
            command.SessionDate,
            command.StartTime,
            command.DurationMinutes,
            command.CapacityLimit,
            command.Description);

        if (sessionResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                sessionResult.Error);
        }

        var session = sessionResult.Value;

        _classSessionRepository.Add(session);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid, Error>(session.Id);
    }

    private static bool IsOverlapping(
        TimeOnly newStart,
        TimeOnly newEnd,
        TimeOnly existingStart,
        TimeOnly existingEnd)
    {
        return newStart < existingEnd &&
               existingStart < newEnd;
    }
}