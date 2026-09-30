using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Dashboard.Contracts.Responses;
using TitanFitness.Domain;

namespace TitanFitness.Application.Dashboard.Queries;

public class GetTodayCheckInsQuery   : IRequest<Result<TodayCheckInsResponse, Error>>
{

}
