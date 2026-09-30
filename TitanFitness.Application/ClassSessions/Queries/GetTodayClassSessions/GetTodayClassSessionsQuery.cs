using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.ClassSessions.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.ClassSessions.Queries.GetTodayClassSessions;

public sealed record GetTodayClassSessionsQuery(
    Guid? BranchId)
    : IRequest<Result<List<TodayClassSessionResponse>, Error>>;