using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Application.Trainers.Commands.CreateTrainer;

public sealed class CreateTrainerCommandHandler
    : IRequestHandler<CreateTrainerCommand, Result<Guid, Error>>
{
    private readonly IReadRepository<Branch> _branchRepository;
    private readonly IRepository<Trainer> _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTrainerCommandHandler(
        IReadRepository<Branch> branchRepository,
        IRepository<Trainer> trainerRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _trainerRepository = trainerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        CreateTrainerCommand command,
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

        var trainerResult = Trainer.Create(
            command.TrainerNumber,
            command.TrainerName,
            email,
            phone,
            command.IsActive,
            command.BranchId);

        if (trainerResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                trainerResult.Error);
        }

        var trainer = trainerResult.Value;

        _trainerRepository.Add(trainer);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success<Guid, Error>(
            trainer.Id);
    }
}