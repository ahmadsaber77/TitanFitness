using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Application.Trainers.Commands.UpdateTrainer;

public sealed class UpdateTrainerCommandHandler
    : IRequestHandler<UpdateTrainerCommand, Result<Guid, Error>>
{
    private readonly IRepository<Trainer> _trainerRepository;
    private readonly IReadRepository<Branch> _branchRepository;
    private readonly IReadRepository<ClassSession> _classSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTrainerCommandHandler(
        IRepository<Trainer> trainerRepository,
        IReadRepository<Branch> branchRepository,
        IReadRepository<ClassSession> classSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _trainerRepository = trainerRepository;
        _branchRepository = branchRepository;
        _classSessionRepository = classSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        UpdateTrainerCommand command,
        CancellationToken cancellationToken)
    {
        var trainer = await _trainerRepository.GetByIdAsync(
            command.TrainerId,
            cancellationToken);

        if (trainer is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Trainer>(command.TrainerId));
        }

        var branch = await _branchRepository.GetByIdAsync(
            command.BranchId,
            cancellationToken);

        if (branch is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Branch>(command.BranchId));
        }

        Email? email = null;

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var emailResult = Email.Create(command.Email);

            if (emailResult.IsFailure)
            {
                return Result.Failure<Guid, Error>(
                    emailResult.Error);
            }

            email = emailResult.Value;
        }

        Phone? phone = null;

        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var phoneResult = Phone.Create(command.Phone);

            if (phoneResult.IsFailure)
            {
                return Result.Failure<Guid, Error>(
                    phoneResult.Error);
            }

            phone = phoneResult.Value;
        }

        if (trainer.IsActive && !command.IsActive)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

            var hasFutureSessions =
                await _classSessionRepository
                    .Query()
                    .AnyAsync(
                        x =>
                            x.TrainerId == trainer.Id
                            && x.Status != ClassSessionStatus.Cancelled
                            && (
                                x.SessionDate > today
                                || (
                                    x.SessionDate == today
                                    && x.StartTime > currentTime
                                )
                            ),
                        cancellationToken);

            if (hasFutureSessions)
            {
                return Result.Failure<Guid, Error>(
                    Error.Conflict<Trainer>(
                        "Trainer cannot be deactivated because they have scheduled sessions."));
            }
        }

        var updateResult = trainer.Update(
            command.TrainerName,
            email,
            phone,
            command.IsActive,
            command.BranchId);

        if (updateResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid, Error>(
            trainer.Id);
    }
}