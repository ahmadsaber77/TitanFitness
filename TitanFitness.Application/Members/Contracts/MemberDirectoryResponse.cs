using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Members.Contracts;

public sealed record MemberDirectoryResponse(
    Guid Id,
    string FullName,
    string MembershipNumber,
    MembershipStatus Status,
    string BranchName,
    DateTime? LastVisit);
