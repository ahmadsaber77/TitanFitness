using TitanFitness.Domain.Common;

namespace TitanFitness.Domain.Enums;

public sealed class CheckInResult : Enumeration
{
    private CheckInResult(int id, string name)
        : base(id, name)
    {
    }

    public static readonly CheckInResult Admitted =
        new(1, "Admitted");

    public static readonly CheckInResult Refused =
        new(2, "Refused");
}