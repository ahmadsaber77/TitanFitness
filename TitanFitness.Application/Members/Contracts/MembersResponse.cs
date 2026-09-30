using TitanFitness.Application.Common;

namespace TitanFitness.Application.Members.Contracts;

public sealed record MembersResponse(
    List<MemberDirectoryResponse> Items,
   PaginationResponse Pagination);



