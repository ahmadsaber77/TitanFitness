using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Trainers.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Trainers.Queries.GetTrainers;

public sealed record GetTrainersQuery(
    string? Search,
    Guid? BranchId,
    int Page,
    int PageSize)
    : IRequest<Result<TrainerDirectoryResponse, Error>>;




