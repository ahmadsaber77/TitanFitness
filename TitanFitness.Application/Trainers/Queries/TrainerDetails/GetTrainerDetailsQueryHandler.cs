using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Trainers.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Trainers.Queries.TrainerDetails;

public sealed class GetTrainerDetailsQueryHandler
    : IRequestHandler<
        GetTrainerDetailsQuery,
        Result<TrainerDetailsResponse, Error>>
{
    private readonly IReadRepository<Trainer> _trainerRepository;
    private readonly IReadRepository<Branch> _branchRepository;

    public GetTrainerDetailsQueryHandler(
        IReadRepository<Trainer> trainerRepository,
        IReadRepository<Branch> branchRepository)
    {
        _trainerRepository = trainerRepository;
        _branchRepository = branchRepository;
    }

    public async Task<Result<TrainerDetailsResponse, Error>> Handle(
        GetTrainerDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var trainer = await _trainerRepository.GetByIdAsync(
            query.TrainerId,
            cancellationToken);

        if (trainer is null)
        {
            return Result.Failure<TrainerDetailsResponse, Error>(
                Error.NotFound<Trainer>(query.TrainerId));
        }

        var branch = await _branchRepository.GetByIdAsync(
            trainer.BranchId,
            cancellationToken);

        if (branch is null)
        {
            return Result.Failure<TrainerDetailsResponse, Error>(
                Error.NotFound<Branch>(trainer.BranchId));
        }

        var response = new TrainerDetailsResponse(
            trainer.Id,
            trainer.TrainerNumber,
            trainer.TrainerName,
            branch.Name,
            trainer.Email?.Value,
            trainer.Phone?.Value,
            trainer.IsActive);

        return Result.Success<TrainerDetailsResponse, Error>(
            response);
    }
}