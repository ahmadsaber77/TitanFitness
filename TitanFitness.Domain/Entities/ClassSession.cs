using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Domain.Entities;

public  class ClassSession : IAggregateRoot 
{

    private ClassSession()
    { 
    }


    private ClassSession(
    Guid id,
    string className,
    Guid branchId,
    Guid studioId,
    Guid trainerId,
    DateOnly sessionDate,
    TimeOnly startTime,
    int durationMinutes,
    int capacityLimit,
    ClassSessionStatus status,
    string? description)
    {
        Id = id;
        ClassName = className;
        BranchId = branchId;
        StudioId = studioId;
        TrainerId = trainerId;
        SessionDate = sessionDate;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        CapacityLimit = capacityLimit;
        Status = status;
        Description = description;
    }



    public static Result<ClassSession, Error> Create(
    string className,
    Guid branchId,
    Guid studioId,
    Guid trainerId,
    DateOnly sessionDate,
    TimeOnly startTime,
    int durationMinutes,
    int capacityLimit,
    string? description)
    {
        
            if (string.IsNullOrWhiteSpace(className))
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Class name is required."));

            if (className.Length > 100)
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Class name cannot exceed 100 characters."));

            if (branchId == Guid.Empty)
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Branch is required."));

            if (studioId == Guid.Empty)
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Studio is required."));

            if (trainerId == Guid.Empty)
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Trainer is required."));

            if (sessionDate == default)
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Session date is required."));

            if (durationMinutes is not (30 or 45 or 60))
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Duration must be 30, 45, or 60 minutes."));

            if (capacityLimit <= 0)
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Capacity limit must be greater than zero."));

            if (description is not null && description.Length > 500)
                return CSharpFunctionalExtensions.Result.Failure<ClassSession, Error>(
                    Error.Validation<ClassSession>(
                        "Description cannot exceed 500 characters."));

            var classSession = new ClassSession(
                Guid.NewGuid(),
                className,
                branchId,
                studioId,
                trainerId,
                sessionDate,
                startTime,
                durationMinutes,
                capacityLimit,
                ClassSessionStatus.Open,
                description);

            return CSharpFunctionalExtensions.Result.Success<ClassSession, Error>(
                classSession);
        


    }


    public Result<ClassSession, Error> Update(
    string className,
    Guid branchId,
    Guid studioId,
    Guid trainerId,
    DateOnly sessionDate,
    TimeOnly startTime,
    int durationMinutes,
    int capacityLimit,
    string? description)
    {
        if (string.IsNullOrWhiteSpace(className))
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Class name is required."));

        if (className.Length > 100)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Class name cannot exceed 100 characters."));

        if (branchId == Guid.Empty)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Branch is required."));

        if (studioId == Guid.Empty)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Studio is required."));

        if (trainerId == Guid.Empty)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Trainer is required."));

        if (sessionDate == default)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Session date is required."));

        if (durationMinutes is not (30 or 45 or 60))
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Duration must be 30, 45, or 60 minutes."));

        if (capacityLimit <= 0)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Capacity limit must be greater than zero."));

        if (description is not null && description.Length > 500)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Description cannot exceed 500 characters."));

        ClassName = className;
        BranchId = branchId;
        StudioId = studioId;
        TrainerId = trainerId;
        SessionDate = sessionDate;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        CapacityLimit = capacityLimit;
        Description = description;

        return Result.Success<ClassSession, Error>(this);
    }



    public Result<ClassSession, Error> UpdateStatus(
    DateTime currentDateTime)
    {
        if (currentDateTime == default)
            return Result.Failure<ClassSession, Error>(
                Error.Validation<ClassSession>(
                    "Current date and time is required."));

        if (Status == ClassSessionStatus.Cancelled)
            return Result.Success<ClassSession, Error>(this);

        var sessionStart = SessionDate.ToDateTime(StartTime);

        var sessionEnd = sessionStart.AddMinutes(DurationMinutes);

        if (currentDateTime < sessionStart)
        {
            Status = ClassSessionStatus.Open;
        }
        else if (currentDateTime < sessionEnd)
        {
            Status = ClassSessionStatus.InProgress;
        }
        else
        {
            Status = ClassSessionStatus.Completed;
        }

        return Result.Success<ClassSession, Error>(this);
    }


    public Result<ClassSession, Error> Cancel()
    {
        if (Status == ClassSessionStatus.Cancelled)
            return Result.Failure<ClassSession, Error>(
                Error.Conflict<ClassSession>(
                    "Class session is already cancelled."));

        if (Status != ClassSessionStatus.Open)
            return Result.Failure<ClassSession, Error>(
                Error.Conflict<ClassSession>(
                    "Only open class sessions can be cancelled."));

        Status = ClassSessionStatus.Cancelled;

        return Result.Success<ClassSession, Error>(this);
    }



    //public Result<Booking, Error> AddBooking(
    //  Guid memberId,
    //  DateTime bookedOn,
    //  string? trainerNotes)
    //{
    //    if (Status != ClassSessionStatus.Open)
    //    {
    //        return Result.Failure<Booking, Error>(
    //            Error.Conflict<ClassSession>(
    //                "Bookings are allowed only for open sessions."));
    //    }

    //    if (memberId == Guid.Empty)
    //    {
    //        return Result.Failure<Booking, Error>(
    //            Error.Validation<Booking>(
    //                "Member is required."));
    //    }

    //    if (bookedOn == default)
    //    {
    //        return Result.Failure<Booking, Error>(
    //            Error.Validation<Booking>(
    //                "Booking date is required."));
    //    }

    //    if (trainerNotes is not null && trainerNotes.Length > 500)
    //    {
    //        return Result.Failure<Booking, Error>(
    //            Error.Validation<Booking>(
    //                "Trainer notes cannot exceed 500 characters."));
    //    }

    //    if (_bookings.Any(x =>
    //        x.MemberId == memberId &&
    //        x.Status != BookingStatus.Cancelled))
    //    {
    //        return Result.Failure<Booking, Error>(
    //            Error.Conflict<ClassSession>(
    //                "Member already has a booking for this session."));
    //    }

    //    var bookedCount = _bookings.Count(x =>
    //        x.Status == BookingStatus.Booked);

    //    if (bookedCount < CapacityLimit)
    //    {
    //        var bookingResult = Booking.CreateBooked(
    //            Id,
    //            memberId,
    //            bookedOn,
    //            trainerNotes);

    //        if (bookingResult.IsFailure)
    //        {
    //            return Result.Failure<Booking, Error>(
    //                bookingResult.Error);
    //        }

    //        _bookings.Add(bookingResult.Value);

    //        return Result.Success<Booking, Error>(
    //            bookingResult.Value);
    //    }

    //    var lastWaitlistPosition = _bookings
    //        .Where(x => x.Status == BookingStatus.Waitlisted)
    //        .Select(x => x.WaitlistPosition)
    //        .Where(x => x.HasValue)
    //        .Select(x => x!.Value)
    //        .DefaultIfEmpty(0)
    //        .Max();

    //    var nextWaitlistPosition = lastWaitlistPosition + 1;

    //    var waitlistedBookingResult = Booking.CreateWaitlisted(
    //        Id,
    //        memberId,
    //        bookedOn,
    //        nextWaitlistPosition,
    //        trainerNotes);

    //    if (waitlistedBookingResult.IsFailure)
    //    {
    //        return Result.Failure<Booking, Error>(
    //            waitlistedBookingResult.Error);
    //    }

    //    _bookings.Add(waitlistedBookingResult.Value);

    //    return Result.Success<Booking, Error>(
    //        waitlistedBookingResult.Value);
    //}


    public  Guid Id { get; private set; }
    public string ClassName { get; private set; } = string.Empty;

    public Guid BranchId { get; private set; }

    public Guid StudioId { get; private set; }

    public Guid TrainerId { get; private set; }

    public DateOnly SessionDate { get; private set; }

    public TimeOnly StartTime { get; private set; }

    public int DurationMinutes { get; private set; }

    public int CapacityLimit { get; private set; }

    public ClassSessionStatus Status { get; private set; }

    public string? Description { get; private set; }


    private readonly List<Booking> _bookings = new();
    public IReadOnlyCollection<Booking> Bookings => _bookings;
}
