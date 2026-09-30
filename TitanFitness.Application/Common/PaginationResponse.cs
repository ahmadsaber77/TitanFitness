namespace TitanFitness.Application.Common;

public sealed record PaginationResponse(
    int Page,
    int PageSize,
    int TotalCount);
