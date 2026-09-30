using Azure;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TitanFitness.Application.Dashboard.Contracts.Responses;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Dashboard.Queries;

public sealed class GetUpcomingClassesQueryHandler
    : IRequestHandler<
        GetUpcomingClassesQuery,
        Result<List<UpcomingClassResponse>, Error>>
{
    private readonly IReadRepository<ClassSession> _classSessionRepository;
    private readonly IReadRepository<Studio> _studioRepository;
    private readonly IReadRepository<Trainer> _trainerRepository;

    public GetUpcomingClassesQueryHandler(
        IReadRepository<ClassSession> classSessionRepository,
        IReadRepository<Studio> studioRepository,
        IReadRepository<Trainer> trainerRepository)
    {
        _classSessionRepository = classSessionRepository;
        _studioRepository = studioRepository;
        _trainerRepository = trainerRepository;
    }

    public async Task<Result<List<UpcomingClassResponse>, Error>> Handle(
        GetUpcomingClassesQuery query,
        CancellationToken cancellationToken)
    {
        var sessions = _classSessionRepository.Query();

        var now = DateTime.UtcNow;

        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        var upcomingSessions = sessions
            .Where(x =>
                x.SessionDate > today ||
                (x.SessionDate == today &&
                 x.StartTime.AddMinutes(x.DurationMinutes) > currentTime))
            .OrderBy(x => x.SessionDate)
            .ThenBy(x => x.StartTime)
            .Take(2)
            .AsNoTracking();

        var sessionList = await upcomingSessions
            .ToListAsync(cancellationToken);

        var responses = new List<UpcomingClassResponse>();

        foreach (var session in sessionList)
        {
            var studio = await _studioRepository.GetByIdAsync(
                session.StudioId,
                cancellationToken);

            var trainer = await _trainerRepository.GetByIdAsync(
                session.TrainerId,
                cancellationToken);

            var placesTaken = session.Bookings
                .Count(x =>
                    x.Status == BookingStatus.Booked ||
                    x.Status == BookingStatus.Attended ||
                     x.Status == BookingStatus.NoShow);

            var status = session.SessionDate == today &&
                         session.StartTime <= currentTime
                ? "Running"
                : "Upcoming";

            var response = new UpcomingClassResponse(
                session.Id,
                session.SessionDate,
                session.StartTime,
                studio!.Name,
                trainer!.TrainerName,
                placesTaken,
                session.CapacityLimit,
                status);

            responses.Add(response);
        }

        return Result.Success<List<UpcomingClassResponse>, Error>(
            responses);
    }
}



