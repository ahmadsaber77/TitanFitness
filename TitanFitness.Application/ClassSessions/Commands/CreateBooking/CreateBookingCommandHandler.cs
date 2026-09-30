using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.ClassSessions.Commands.CreateBooking;

public sealed class CreateBookingCommandHandler
    : IRequestHandler<
        CreateBookingCommand,
        Result<Guid, Error>>
{
    private readonly IReadRepository<Member> _memberRepository;
    private readonly IReadRepository<Membership> _membershipRepository;
    private readonly IReadRepository<ClassSession> _classSessionReadRepository;
    private readonly IReadRepository<Booking> _bookingRepository;
    private readonly IRepository<ClassSession> _classSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(
        IReadRepository<Member> memberRepository,
        IReadRepository<Membership> membershipRepository,
        IReadRepository<ClassSession> classSessionReadRepository,
        IReadRepository<Booking> bookingRepository,
        IRepository<ClassSession> classSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _membershipRepository = membershipRepository;
        _classSessionReadRepository = classSessionReadRepository;
        _bookingRepository = bookingRepository;
        _classSessionRepository = classSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, Error>> Handle(
        CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(
            command.MemberId,
            cancellationToken);

        if (member is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<Member>(command.MemberId));
        }

        var membership = await _membershipRepository.FindAsync(
            x => x.MemberId == command.MemberId &&
                 x.Status == MembershipStatus.Active,
            cancellationToken);

        if (membership is null)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<Member>(
                    "Member does not have an active membership."));
        }

        var session = await _classSessionReadRepository.GetByIdAsync(
            command.SessionId,
            cancellationToken);

        if (session is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<ClassSession>(command.SessionId));
        }

        var now = DateTime.UtcNow;

        if (session.Status == ClassSessionStatus.Cancelled)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "Cancelled session cannot accept bookings."));
        }

        var sessionStart = session.SessionDate.ToDateTime(
            session.StartTime);

        var sessionEnd = sessionStart.AddMinutes(
            session.DurationMinutes);

        if (now >= sessionStart)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "Session no longer accepts bookings."));
        }

        var hasOverlappingBooking =
            await HasOverlappingBookingAsync(
                command.MemberId,
                session,
                cancellationToken);

        if (hasOverlappingBooking)
        {
            return Result.Failure<Guid, Error>(
                Error.Conflict<ClassSession>(
                    "Member already has an overlapping session."));
        }

        var trackedSession = await _classSessionRepository.GetByIdAsync(
            command.SessionId,
            cancellationToken);

        if (trackedSession is null)
        {
            return Result.Failure<Guid, Error>(
                Error.NotFound<ClassSession>(command.SessionId));
        }

        var bookingResult = trackedSession.AddBooking(
            command.MemberId,
            now,
            command.TrainerNotes);

        if (bookingResult.IsFailure)
        {
            return Result.Failure<Guid, Error>(
                bookingResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid, Error>(
            bookingResult.Value.Id);
    }

    private async Task<bool> HasOverlappingBookingAsync(
        Guid memberId,
        ClassSession newSession,
        CancellationToken cancellationToken)
    {
        var existingBookings = await _bookingRepository
            .Query()
            .Where(x =>
                x.MemberId == memberId &&
                x.Status != BookingStatus.Cancelled)
            .Select(x => new
            {
                x.SessionId
            })
            .ToListAsync(cancellationToken);

        if (existingBookings.Count == 0)
        {
            return false;
        }

        var existingSessionIds = existingBookings
            .Select(x => x.SessionId)
            .Distinct()
            .ToList();

        var existingSessions = await _classSessionReadRepository
            .Query()
            .Where(x =>
                existingSessionIds.Contains(x.Id) &&
                x.Status != ClassSessionStatus.Cancelled)
            .ToListAsync(cancellationToken);

        var newStart = newSession.SessionDate.ToDateTime(
            newSession.StartTime);

        var newEnd = newStart.AddMinutes(
            newSession.DurationMinutes);

        return existingSessions.Any(existingSession =>
        {
            var existingStart =
                existingSession.SessionDate.ToDateTime(
                    existingSession.StartTime);

            var existingEnd = existingStart.AddMinutes(
                existingSession.DurationMinutes);

            return newStart < existingEnd &&
                   existingStart < newEnd;
        });
    }
}