using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Application.Common;
using TitanFitness.Application.Trainers.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Trainers.Queries.GetTrainers;


public sealed class GetTrainersQueryHandler
  : IRequestHandler<GetTrainersQuery, Result<TrainerDirectoryResponse, Error>>
{
    private readonly IReadRepository<Trainer> _trainerRepository;
    private readonly IReadRepository<Branch> _branchRepository;

    public GetTrainersQueryHandler(
        IReadRepository<Trainer> trainerRepository,
        IReadRepository<Branch> branchRepository)
    {
        _trainerRepository = trainerRepository;
        _branchRepository = branchRepository;
    }

    public async Task<Result<TrainerDirectoryResponse, Error>> Handle(
        GetTrainersQuery query,
        CancellationToken cancellationToken)
    {
        var trainersQuery =
            from trainer in _trainerRepository.Query()
            join branch in _branchRepository.Query()
                on trainer.BranchId equals branch.Id
            select new
            {
                Trainer = trainer,
                BranchName = branch.Name
            };

        if (query.BranchId.HasValue)
        {
            trainersQuery = trainersQuery.Where(x =>
                x.Trainer.BranchId == query.BranchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            trainersQuery = trainersQuery.Where(x =>
                x.Trainer.TrainerName.Contains(search) ||
                x.Trainer.TrainerNumber.Contains(search) ||
                x.BranchName.Contains(search) ||
                (x.Trainer.IsActive && "Active".Contains(search)) ||
                (!x.Trainer.IsActive && "Inactive".Contains(search)));
        }

        var totalCount = await trainersQuery.CountAsync(cancellationToken);

        var trainers = await trainersQuery
            .OrderBy(x => x.Trainer.TrainerName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = trainers
            .Select(x =>
                new TrainerDirectoryResponse.TrainerDirectoryItemResponse(
                    x.Trainer.Id,
                    x.Trainer.TrainerNumber,
                    x.Trainer.TrainerName,
                    x.BranchName,
                    x.Trainer.IsActive))
            .ToList();

        var pagination = new PaginationResponse(
            query.Page,
            query.PageSize,
            totalCount);

        var response = new TrainerDirectoryResponse(
            items,
            pagination);

        return Result.Success<TrainerDirectoryResponse, Error>(
            response);
    }
}
