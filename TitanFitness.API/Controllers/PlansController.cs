using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TitanFitness.Application.Plans.Commands.CreatePlan;
using TitanFitness.Application.Plans.Commands.UpdatePlan;
using TitanFitness.Application.Plans.Contracts;
using TitanFitness.Application.Plans.Queries.GetPlanDetails;
using TitanFitness.Domain;
using TitanFitness.WebAPI.Contracts.Plans;

namespace TitanFitness.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlansController : ControllerBase
{

    private readonly ISender _sender;

 

    public PlansController(
     ISender sender)
    {
        _sender = sender;
       
    }

    [HttpGet("{planId:guid}")]
    [ProducesResponseType(typeof(PlanDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType( typeof(Error),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlan(
        Guid planId,
        CancellationToken cancellationToken)
    {
        if (planId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "PlanId is required."
            });
        }

        var query = new GetPlanDetailsQuery(
            planId);

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


    [HttpPost][ProducesResponseType(typeof(Guid),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlan(
    [FromBody] CreatePlanRequest request,
    CancellationToken cancellationToken)
    {
      

        var command = new CreatePlanCommand(
            request.PlanName,
            request.Price,
            request.DurationInMonths,
            request.IsPublished,
            request.MaxFreezeDays,
            request.MaxNumberOfFreezes,
            request.GuestPassQuota,
            request.AccessScope);

        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Created(
                $"/api/plans/{result.Value}",
                result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }



    [HttpPut("{planId:guid}")] [ProducesResponseType( typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error),StatusCodes.Status400BadRequest)]
    [ProducesResponseType( typeof(Error),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlan(
    Guid planId,
    [FromBody] UpdatePlanRequest request,
    CancellationToken cancellationToken)
    {
        if (planId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "PlanId is required."
            });
        }

        

        var command = new UpdatePlanCommand(
            planId,
            request.PlanName,
            request.Price,
            request.DurationInMonths,
            request.IsPublished,
            request.MaxFreezeDays,
            request.MaxNumberOfFreezes,
            request.GuestPassQuota,
            request.AccessScope);

        var result = await _sender.Send(
            command,
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
