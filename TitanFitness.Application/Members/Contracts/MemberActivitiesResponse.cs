namespace TitanFitness.Application.Members.Contracts;

public sealed record MemberActivityResponse(
    string Type,
    DateTime DateTime,
    string Description);

public sealed record MemberActivitiesResponse(
    List<MemberActivityResponse> Items);