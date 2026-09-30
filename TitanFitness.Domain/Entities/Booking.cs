using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Domain.Entities;

public class Booking  :IEntity 
{
    private Booking()
    { 
    }

  

    private Booking(
        Guid id,
        Guid sessionId,
        Guid memberId,
        DateTime bookedOn,
        int? waitlistPosition,
        string? trainerNotes)
    {
        Id = id;
        SessionId = sessionId;
        MemberId = memberId;
        BookedOn = bookedOn;
        Status = BookingStatus.Booked;
        WaitlistPosition = waitlistPosition;
        TrainerNotes = trainerNotes;
    }

    internal static Result<Booking, Error> CreateBooked(
     Guid sessionId,
     Guid memberId,
     DateTime bookedOn,
     int? waitlistPosition,
     string? trainerNotes)
    {
        if (sessionId == Guid.Empty)
        {
            return Result.Failure<Booking, Error>(
                Error.Validation<Booking>(
                    "SessionId is required."));
        }

        if (memberId == Guid.Empty)
        {
            return Result.Failure<Booking, Error>(
                Error.Validation<Booking>(
                    "MemberId is required."));
        }

        if (bookedOn == default)
        {
            return Result.Failure<Booking, Error>(
                Error.Validation<Booking>(
                    "BookedOn is required."));
        }

        if (waitlistPosition.HasValue && waitlistPosition.Value <= 0)
        {
            return Result.Failure<Booking, Error>(
                Error.Validation<Booking>(
                    "Waitlist position must be greater than zero."));
        }

        if (trainerNotes is not null && trainerNotes.Length > 500)
        {
            return Result.Failure<Booking, Error>(
                Error.Validation<Booking>(
                    "Trainer notes cannot exceed 500 characters."));
        }

        var booking = new Booking(
            Guid.NewGuid(),
            sessionId,
            memberId,
            bookedOn,
            waitlistPosition,
            trainerNotes);

        booking.Status = waitlistPosition.HasValue
            ? BookingStatus.Waitlisted
            : BookingStatus.Booked;

        return Result.Success<Booking, Error>(booking);
    }




    public Result<Booking, Error> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("Booking is already cancelled."));

        if (Status == BookingStatus.Attended)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("An attended booking cannot be cancelled."));

        if (Status == BookingStatus.NoShow)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("A no-show booking cannot be cancelled."));

        Status = BookingStatus.Cancelled;
        WaitlistPosition = null;

        return Result.Success<Booking, Error>(this);
    }

    public Result<Booking, Error> MarkAttended()
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("A cancelled booking cannot be marked as attended."));

        if (Status == BookingStatus.Attended)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("Booking is already marked as attended."));

        if (Status == BookingStatus.NoShow)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("A no-show booking cannot be marked as attended."));

        if (Status == BookingStatus.Waitlisted)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("A waitlisted booking cannot be marked as attended."));

        Status = BookingStatus.Attended;
        WaitlistPosition = null;

        return Result.Success<Booking, Error>(this);
    }

    public Result<Booking, Error> MarkNoShow()
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("A cancelled booking cannot be marked as no-show."));

        if (Status == BookingStatus.Attended)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("An attended booking cannot be marked as no-show."));

        if (Status == BookingStatus.NoShow)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("Booking is already marked as no-show."));

        if (Status == BookingStatus.Waitlisted)
            return Result.Failure<Booking, Error>(
                Error.Conflict<Booking>("A waitlisted booking cannot be marked as no-show."));

        Status = BookingStatus.NoShow;
        WaitlistPosition = null;

        return Result.Success<Booking, Error>(this);
    }

    public Guid Id { get; private set; }

    public Guid SessionId { get; private set; }

    public Guid MemberId { get; private set; }

    public DateTime BookedOn { get; private set; }

    public BookingStatus Status { get; private set; }

    public int? WaitlistPosition { get; private set; }

    public string? TrainerNotes { get; private set; }

}
