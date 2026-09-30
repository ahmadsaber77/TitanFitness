using MediatR;
using Microsoft.AspNetCore.Mvc;
using TitanFitness.Application.Freezes.Commands.FreezeMembership;
using TitanFitness.Application.Freezes.Contracts;
using TitanFitness.Domain;

namespace TitanFitness.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FreezesController : ControllerBase
{
    private readonly ISender _sender;

    public FreezesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{memberId:guid}/membership/freeze")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> FreezeMembership(
    Guid memberId,
    [FromBody] FreezeMembershipRequest request,
    CancellationToken cancellationToken)
    {
        if (memberId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "MemberId is required."
            });
        }

        var command = new FreezeMembershipCommand(
            memberId,
            request.StartDate,
            request.DurationInMonths,
            request.Reason,
            request.AdditionalNotes);

        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Created(
                $"/api/memberships/freezes/{result.Value}",
                result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }




}
