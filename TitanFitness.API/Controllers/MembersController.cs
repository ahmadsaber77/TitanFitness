using MediatR;
using Microsoft.AspNetCore.Mvc;
using TitanFitness.Application.Members.Commands.ChangeMembershipPlan.ChangeMembershipPlanAtRenewal;
using TitanFitness.Application.Members.Commands.ChangeMembershipPlan.ChangeMembershipPlanImmediately;
using TitanFitness.Application.Members.Commands.CreateMember;
using TitanFitness.Application.Members.Commands.UpdateMember;
using TitanFitness.Application.Members.Contracts;
using TitanFitness.Application.Members.Queries.GetMemberActivities;
using TitanFitness.Application.Members.Queries.GetMemberFreezeSummary;
using TitanFitness.Application.Members.Queries.GetMemberGuestPassSummary;
using TitanFitness.Application.Members.Queries.GetMemberMembership;
using TitanFitness.Application.Members.Queries.GetMemberProfile;
using TitanFitness.Application.Members.Queries.GetMembers;
using TitanFitness.Domain;

namespace TitanFitness.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MembersController : ControllerBase
{
    private readonly ISender _sender;

    public MembersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMembers(
     [FromQuery] string? search,
     [FromQuery] Guid? branchId,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10,
     CancellationToken cancellationToken = default)
    {
        if (branchId.HasValue && branchId.Value == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "BranchId must be a valid identifier."
            });
        }

        if (page < 1)
        {
            return BadRequest(new
            {
                Message = "Page must be greater than 0."
            });
        }

        if (pageSize < 1)
        {
            return BadRequest(new
            {
                Message = "PageSize must be greater than 0."
            });
        }

        var query = new GetMembersQuery(
            search,
            branchId,
            page,
            pageSize);

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


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMember(
    [FromBody] CreateMemberCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Created(
                $"/api/members/{result.Value}",
                result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }



    [HttpPut("{memberId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMember(
    Guid memberId,
    [FromBody] UpdateMemberRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateMemberCommand(
            memberId,
            request.FullName,
            request.Email,
            request.Phone,
            request.Address,
            request.JoinedDate,
            request.HomeBranchId,
            request.Photo);

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



    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{memberId:guid}")]
    public async Task<IActionResult> GetMember(
    Guid memberId,
    CancellationToken cancellationToken)
    {
        var query = new GetMemberQuery(memberId);

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



    [HttpGet("{memberId:guid}/membership")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberMembership(
     Guid memberId,
     CancellationToken cancellationToken)
    {
        if (memberId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "MemberId is required."
            });
        }

        var query = new GetMemberMembershipQuery(memberId);

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


    [HttpGet("{memberId:guid}/freezes/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberFreezeSummary(
    Guid memberId,
    CancellationToken cancellationToken)
    {
        if (memberId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "MemberId is required."
            });
        }

        var query = new GetMemberFreezeSummaryQuery(memberId);

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



    [HttpGet("{memberId:guid}/guest-passes/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberGuestPassSummary(
    Guid memberId,
    CancellationToken cancellationToken)
    {
        if (memberId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "MemberId is required."
            });
        }

        var query = new GetMemberGuestPassSummaryQuery(memberId);

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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{memberId:guid}/activities")]
    public async Task<IActionResult> GetMemberActivities(
    Guid memberId,
    CancellationToken cancellationToken)
    {
        if (memberId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "MemberId is required."
            });
        }

        var query = new GetMemberActivitiesQuery(memberId);

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



    [HttpPost("{memberId:guid}/membership/change-plan/renewal")]
    public async Task<IActionResult> ChangeMembershipPlanAtRenewal(
    Guid memberId,
    [FromBody] ChangeMembershipPlanAtRenewalRequest request,
    CancellationToken cancellationToken)
    {
        if (memberId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "MemberId is required."
            });
        }

        if (request.PlanId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "PlanId is required."
            });
        }

        var command = new ChangeMembershipPlanAtRenewalCommand(
            memberId,
            request.PlanId);

        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Created(
                $"/api/memberships/{result.Value}",
                result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }




    [HttpPost("{memberId:guid}/membership/change-plan/immediately")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeMembershipPlanImmediately(
     Guid memberId,
     [FromBody] ChangeMembershipPlanImmediatelyRequest request,
     CancellationToken cancellationToken)
    {
        if (memberId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "MemberId is required."
            });
        }

        if (request.PlanId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "PlanId is required."
            });
        }

        var command = new ChangeMembershipPlanImmediatelyCommand(
            memberId,
            request.PlanId);

        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Created(
                $"/api/memberships/{result.Value}",
                result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }
}
