using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Application.ClassSessions.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.ClassSessions.Queries.GetTodayClassSessions;

public sealed class GetTodayClassSessionsQueryHandler
    : IRequestHandler<
        GetTodayClassSessionsQuery,
        Result<List<TodayClassSessionResponse>, Error>>
{
    private readonly IReadRepository<ClassSession> _classSessionRepository;
    private readonly IReadRepository<Trainer> _trainerRepository;
    private readonly IReadRepository<Studio> _studioRepository;
    private readonly IReadRepository<Booking> _bookingRepository;

    public GetTodayClassSessionsQueryHandler(
        IReadRepository<ClassSession> classSessionRepository,
        IReadRepository<Trainer> trainerRepository,
        IReadRepository<Studio> studioRepository,
        IReadRepository<Booking> bookingRepository)
    {
        _classSessionRepository = classSessionRepository;
        _trainerRepository = trainerRepository;
        _studioRepository = studioRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<Result<List<TodayClassSessionResponse>, Error>> Handle(
        GetTodayClassSessionsQuery query,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessionsQuery =
            from session in _classSessionRepository.Query()
            join trainer in _trainerRepository.Query()
                on session.TrainerId equals trainer.Id
            join studio in _studioRepository.Query()
                on session.StudioId equals studio.Id
            where session.SessionDate == today
            select new
            {
                Session = session,
                TrainerName = trainer.TrainerName,
                StudioName = studio.Name
            };

        if (query.BranchId.HasValue)
        {
            sessionsQuery = sessionsQuery.Where(x =>
                x.Session.BranchId == query.BranchId.Value);
        }

        var sessions = await sessionsQuery
            .OrderBy(x => x.Session.StartTime)
            .ToListAsync(cancellationToken);

        var sessionIds = sessions
            .Select(x => x.Session.Id)
            .ToList();

        var bookings = await _bookingRepository
            .Query()
            .Where(x => sessionIds.Contains(x.SessionId))
            .Select(x => new
            {
                x.SessionId,
                x.Status
            })
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var response = sessions
            .Select(x =>
            {
                var sessionBookings = bookings
                    .Where(b => b.SessionId == x.Session.Id)
                    .ToList();

                var bookedCount = sessionBookings.Count(b =>
                    b.Status != BookingStatus.Waitlisted &&
                    b.Status != BookingStatus.Cancelled);

                var waitlistCount = sessionBookings.Count(b =>
                    b.Status == BookingStatus.Waitlisted);

                var status = GetCurrentStatus(
                    x.Session,
                    now);

                return new TodayClassSessionResponse(
                    x.Session.Id,
                    x.Session.StartTime,
                    x.Session.ClassName,
                    x.TrainerName,
                    x.StudioName,
                    bookedCount,
                    x.Session.CapacityLimit,
                    waitlistCount,
                    status);
            })
            .ToList();

        return Result.Success<
            List<TodayClassSessionResponse>,
            Error>(response);
    }

    private static ClassSessionStatus GetCurrentStatus(
        ClassSession session,
        DateTime now)
    {
        if (session.Status == ClassSessionStatus.Cancelled)
        {
            return ClassSessionStatus.Cancelled;
        }

        var sessionStart = session.SessionDate.ToDateTime(
            session.StartTime);

        var sessionEnd = sessionStart.AddMinutes(
            session.DurationMinutes);

        if (now < sessionStart)
        {
            return ClassSessionStatus.Open;
        }

        if (now >= sessionEnd)
        {
            return ClassSessionStatus.Completed;
        }

        return ClassSessionStatus.InProgress;
    }
}
