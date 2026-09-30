using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Dashboard.Contracts.Responses;
using TitanFitness.Domain;

namespace TitanFitness.Application.Dashboard.Queries;

public class GetActiveMembersQuery : IRequest<Result<ActiveMembersResponse, Error>>;

