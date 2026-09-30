using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Enums;


namespace TitanFitness.Domain.Entities;

public class CheckIn : IAggregateRoot
{
    private CheckIn() { }

    private CheckIn(
        Guid id,
        Guid memberId,
        Guid branchId,
        DateTime checkInDateTime,
        CheckInResult result,
        string? refusalReason)
    {
        Id = id;
        MemberId = memberId;
        BranchId = branchId;
        CheckInDateTime = checkInDateTime;
        Result = result;
        RefusalReason = refusalReason;
    }

    public static Result<CheckIn, Error> CheackIn(
     Guid memberId,
     Guid branchId,
     DateTime checkInDateTime,
     CheckInResult result,
     string? refusalReason)
    {
        if (memberId == Guid.Empty)
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Validation<CheckIn>(
                    "Member is required."));

        if (branchId == Guid.Empty)
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Validation<CheckIn>(
                    "Branch is required."));

        if (checkInDateTime == default)
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Validation<CheckIn>(
                    "Check-in date and time is required."));

        if (!Enumeration.GetAll<CheckInResult>().Contains(result))
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Validation<CheckIn>(
                    "Invalid check-in result."));

        if (result == CheckInResult.Admitted &&
            !string.IsNullOrWhiteSpace(refusalReason))
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Validation<CheckIn>(
                    "Refusal reason must be empty when check-in is admitted."));

        if (result == CheckInResult.Refused &&
            string.IsNullOrWhiteSpace(refusalReason))
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Validation<CheckIn>(
                    "Refusal reason is required when check-in is refused."));

        if (refusalReason is not null &&
            refusalReason.Length > 100)
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Validation<CheckIn>(
                    "Refusal reason cannot exceed 100 characters."));

        var checkIn = new CheckIn(
            Guid.NewGuid(),
            memberId,
            branchId,
            checkInDateTime,
            result,
            refusalReason);

        return CSharpFunctionalExtensions.Result.Success<CheckIn, Error>(
            checkIn);
    }


    public Result<CheckIn, Error> CheckOut()
    {
        if (Result == CheckInResult.Refused)
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Conflict<CheckIn>(
                    "A refused check in cannot be checked out."));

        if (CheckOutDateTime is not null)
            return CSharpFunctionalExtensions.Result.Failure<CheckIn, Error>(
                Error.Conflict<CheckIn>(
                    "This check in has already been checked out."));

        CheckOutDateTime = DateTime.UtcNow;

        return CSharpFunctionalExtensions.Result.Success<CheckIn, Error>(
            this);
    }


    public Guid Id { get; private set; }

    public Guid MemberId { get; private set; }

    public Guid BranchId { get; private set; }

    public DateTime CheckInDateTime { get; private set; }

    public DateTime? CheckOutDateTime { get; private set; }

    public CheckInResult Result { get; private set; } = null!;
    public string? RefusalReason { get; private set; }
}
