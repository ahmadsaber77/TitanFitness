using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Application.Members.Queries.GetMemberActivities;

public sealed class GetMemberActivitiesQueryHandler
    : IRequestHandler<
        GetMemberActivitiesQuery,
        Result<MemberActivitiesResponse, Error>>
{
    private readonly IReadRepository<Member> _memberRepository;
    private readonly IReadRepository<CheckIn> _checkInRepository;
    private readonly IReadRepository<Booking> _bookingRepository;
    private readonly IReadRepository<ClassSession> _classSessionRepository;

    public GetMemberActivitiesQueryHandler(
        IReadRepository<Member> memberRepository,
        IReadRepository<CheckIn> checkInRepository,
        IReadRepository<Booking> bookingRepository,
        IReadRepository<ClassSession> classSessionRepository)
    {
        _memberRepository = memberRepository;
        _checkInRepository = checkInRepository;
        _bookingRepository = bookingRepository;
        _classSessionRepository = classSessionRepository;
    }

    public async Task<Result<MemberActivitiesResponse, Error>> Handle(
        GetMemberActivitiesQuery query,
        CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(
            query.MemberId,
            cancellationToken);

        if (member is null)
        {
            return Result.Failure<MemberActivitiesResponse, Error>(
                Error.NotFound<Member>(query.MemberId));
        }

        var checkInActivities = await _checkInRepository
            .Query()
            .Where(x => x.MemberId == query.MemberId)
            .OrderByDescending(x => x.CheckInDateTime)
            .Take(7)
            .Select(x => new CheckInActivityData(
                x.CheckInDateTime,
                x.Result))
            .ToListAsync(cancellationToken);

        var classAttendanceActivities = await (
            from booking in _bookingRepository.Query()
            join session in _classSessionRepository.Query()
                on booking.SessionId equals session.Id
            where booking.MemberId == query.MemberId
                  && booking.Status == BookingStatus.Attended
            orderby session.SessionDate descending,
                     session.StartTime descending
            select new ClassAttendanceActivityData(
                session.SessionDate,
                session.StartTime,
                session.ClassName)
        )
        .Take(7)
        .ToListAsync(cancellationToken);

        var activities = checkInActivities
             .Select(x => new MemberActivityResponse(
                        "CheckIn",
                        x.DateTime,
                        x.Result.Name))
                    .Concat(
                        classAttendanceActivities.Select(x =>
                            new MemberActivityResponse(
                                "ClassAttendance",
                                x.SessionDate.ToDateTime(x.StartTime),
                                x.ClassName)))
                    .OrderByDescending(x => x.DateTime)
                    .Take(7)
                    .ToList();

        return Result.Success<MemberActivitiesResponse, Error>(
            new MemberActivitiesResponse(activities));
    }

    private sealed record CheckInActivityData(
        DateTime DateTime,
        CheckInResult Result);

    private sealed record ClassAttendanceActivityData(
        DateOnly SessionDate,
        TimeOnly StartTime,
        string ClassName);
}
