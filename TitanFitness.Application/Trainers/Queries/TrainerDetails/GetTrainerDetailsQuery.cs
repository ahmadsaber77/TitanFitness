using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Trainers.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Trainers.Queries.TrainerDetails;

public sealed record GetTrainerDetailsQuery(
    Guid TrainerId)
    : IRequest<Result<TrainerDetailsResponse, Error>>;