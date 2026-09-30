using CSharpFunctionalExtensions;
using MediatR;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.Application.Members.Queries.GetMembers;

public sealed record GetMembersQuery(
    string? Search,
    Guid? BranchId,
    int Page = 1,
    int PageSize = 10)
    : IRequest<Result<MembersResponse, Error>>;
