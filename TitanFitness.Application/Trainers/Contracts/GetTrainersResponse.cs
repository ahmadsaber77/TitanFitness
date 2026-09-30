using TitanFitness.Application.Common;
using static TitanFitness.Application.Trainers.Contracts.TrainerDirectoryResponse;

namespace TitanFitness.Application.Trainers.Contracts;

public sealed record TrainerDirectoryResponse(
    List<TrainerDirectoryItemResponse> Items,
    PaginationResponse Pagination)


{
    public sealed record TrainerDirectoryItemResponse(
        Guid Id,
        string TrainerNumber,
        string TrainerName,
        string BranchName,
        bool IsActive);
}



