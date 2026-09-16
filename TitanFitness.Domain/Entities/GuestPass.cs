using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;

namespace TitanFitness.Domain.Entities;

public class GuestPass : IEntity
{
    private GuestPass() { }

    private GuestPass(
    Guid id,
    Guid membershipId,
    DateTime issuedOn)
    {
        Id = id;
        MembershipId = membershipId;
        IssuedOn = issuedOn;
    }

    public static Result<GuestPass, Error> Create(
    Guid membershipId,
    DateTime issuedOn)
    {
        if (membershipId == Guid.Empty)
            return Result.Failure<GuestPass, Error>(
                Error.Validation<GuestPass>(
                    "Membership is required."));

        if (issuedOn == default)
            return Result.Failure<GuestPass, Error>(
                Error.Validation<GuestPass>(
                    "Issued date is required."));

        var guestPass = new GuestPass(
            Guid.NewGuid(),
            membershipId,
            issuedOn);

        return Result.Success<GuestPass, Error>(guestPass);
    }


  

    public Guid Id { get; private set; }

    public Guid MembershipId { get; private set; }

    public DateTime IssuedOn { get; private set; }

    public DateTime? UsedOn { get; private set; }

    public string? GuestName { get; private set; }
}
