using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Application.Dashboard.Contracts.Responses;
using TitanFitness.Domain;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Entities;

namespace TitanFitness.Application.Dashboard.Queries;

public class GetTodayCheckInsQueryHandler  :IRequestHandler<
    GetTodayCheckInsQuery,
    Result<TodayCheckInsResponse, Error>>
{

    private readonly IReadRepository<CheckIn> _checkInRepository;

    public GetTodayCheckInsQueryHandler(
        IReadRepository<CheckIn> checkInRepository)
    {
        _checkInRepository = checkInRepository;
    }

          public async Task<Result<TodayCheckInsResponse, Error>> Handle(
        GetTodayCheckInsQuery query,
        CancellationToken cancellationToken)
    {
        var checkIns = _checkInRepository.Query();

        var today = DateTime.UtcNow.Date;
        var lastWeek = today.AddDays(-7);


        var todayCheckIns = checkIns
            .Where(x => x.CheckInDateTime >= today);

        var count = await checkIns
       .CountAsync(
           x => x.CheckInDateTime >= today &&
                x.CheckInDateTime < today.AddDays(1),
           cancellationToken);

        var lastWeekCount = await checkIns
            .CountAsync(
                x => x.CheckInDateTime >= lastWeek &&
                     x.CheckInDateTime < lastWeek.AddDays(1),
                cancellationToken);

        var response = new TodayCheckInsResponse(
            count,
            lastWeekCount);

        return Result.Success<TodayCheckInsResponse, Error>(response);
    }

}



