using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;

namespace TitanFitness.Domain.Entities;

public  class Freeze : IEntity 
{
    private Freeze() { }


    private Freeze(
    Guid id,
    Guid membershipId,
    DateOnly startDate,
    DateOnly endDate,
    int durationInMonths,
    string reason,
    string? additionalNotes,
    DateTime requestedOn)
    {
        Id = id;
        MembershipId = membershipId;
        StartDate = startDate;
        EndDate = endDate;
        DurationInMonths = durationInMonths;
        Reason = reason;
        AdditionalNotes = additionalNotes;
        RequestedOn = requestedOn;
    }


    internal static Result<Freeze, Error> Create(
       Guid membershipId,
       DateOnly startDate,
       DateOnly endDate,
       int durationInMonths,
       string reason,
       string? additionalNotes,
       DateTime requestedOn)
    {
        if (membershipId == Guid.Empty)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>("Membership is required."));

        if (startDate == default)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>("Start date is required."));

        if (endDate == default)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>("End date is required."));

        if (endDate < startDate)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "End date cannot be before start date."));

        if (durationInMonths is < 1 or > 3)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Freeze duration must be 1, 2, or 3 months."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Freeze reason is required."));

        if (additionalNotes is not null && additionalNotes.Length > 200)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Additional notes cannot exceed 200 characters."));

        if (requestedOn == default)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Requested date is required."));

        var freeze = new Freeze(
            Guid.NewGuid(),
            membershipId,
            startDate,
            endDate,
            durationInMonths,
            reason,
            additionalNotes,
            requestedOn);

        return Result.Success<Freeze, Error>(freeze);
    }
    public Guid Id { get; private set; }

    public Guid MembershipId { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public int DurationInMonths { get; private set; }

    public string Reason { get; private set; } = string.Empty;

    public string? AdditionalNotes { get; private set; }

    public DateTime RequestedOn { get; private set; }
}
