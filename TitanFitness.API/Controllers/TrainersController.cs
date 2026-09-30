using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TitanFitness.Application.Trainers.Commands.CreateTrainer;
using TitanFitness.Application.Trainers.Commands.UpdateTrainer;
using TitanFitness.Application.Trainers.Contracts;
using TitanFitness.Application.Trainers.Queries.GetTrainers;
using TitanFitness.Application.Trainers.Queries.TrainerDetails;
using TitanFitness.Domain;

namespace TitanFitness.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrainersController : ControllerBase
{
    private readonly ISender _sender;

    public TrainersController(
        ISender sender)
    {
        _sender = sender;
       
    }

    [HttpGet]
    [ProducesResponseType(typeof(TrainerDirectoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTrainers(
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
                Message = "Page must be greater than zero."
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                Message = "PageSize must be between 1 and 100."
            });
        }

        var query = new GetTrainersQuery(
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

    [HttpGet("{trainerId:guid}")]
    [ProducesResponseType(typeof(TrainerDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType( typeof(Error),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrainer(
        Guid trainerId,
        CancellationToken cancellationToken)
    {
        if (trainerId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "TrainerId is required."
            });
        }

        var query = new GetTrainerDetailsQuery(
            trainerId);

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
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTrainer(
        [FromBody] CreateTrainerRequest request,
        CancellationToken cancellationToken)
    {
        

        var command = new CreateTrainerCommand(
            request.TrainerNumber,
            request.TrainerName,
            request.BranchId,
            request.Email,
            request.Phone,
            request.IsActive);

        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Created(
                $"/api/trainers/{result.Value}",
                result.Value);
        }

        return StatusCode(
            (int)result.Error.StatusCode,
            result.Error);
    }

    [HttpPut("{trainerId:guid}")]
    [ProducesResponseType(typeof(TrainerDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType( typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTrainer(
        Guid trainerId,
        [FromBody] UpdateTrainerRequest request,
        CancellationToken cancellationToken)
    {
        if (trainerId == Guid.Empty)
        {
            return BadRequest(new
            {
                Message = "TrainerId is required."
            });
        }
        var command = new UpdateTrainerCommand(
            trainerId,
            request.TrainerName,
            request.BranchId,
            request.Email,
            request.Phone,
            request.IsActive);

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