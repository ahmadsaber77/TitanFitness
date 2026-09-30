using MediatR;
using Microsoft.AspNetCore.Mvc;
using TitanFitness.Application.Dashboard.Queries;

namespace TitanFitness.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("check-ins-today")]
    public async Task<IActionResult> GetTodayCheckIns(
    CancellationToken cancellationToken)
    {
        var query = new GetTodayCheckInsQuery();

        var result = await _sender.Send(
            query,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }


    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("active-members")]
    public async Task<IActionResult> GetActiveMembers(
    CancellationToken cancellationToken)
    {
        var query = new GetActiveMembersQuery();

        var result = await _sender.Send(
            query,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }


    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("upcoming-classes")]
    public async Task<IActionResult> GetUpcomingClasses(
    CancellationToken cancellationToken)
    {
        var query = new GetUpcomingClassesQuery();

        var result = await _sender.Send(
            query,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }
}
