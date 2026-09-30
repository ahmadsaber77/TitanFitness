using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TitanFitness.Application.ClassSessions.Commands.CreateBooking;
using TitanFitness.Application.ClassSessions.Commands.CreateClassSession;
using TitanFitness.Application.ClassSessions.Contracts;
using TitanFitness.Application.ClassSessions.Queries.GetTodayClassSessions;
using TitanFitness.Domain;

namespace TitanFitness.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSessionsController : ControllerBase
    {

        private readonly ISender _sender;
      


        public ClassSessionsController(
    ISender sender)
        {
            _sender = sender;
            
        }



        [HttpGet("today")]
        [ProducesResponseType(typeof(List<TodayClassSessionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTodayClassSessions(
               [FromQuery] Guid? branchId,
               CancellationToken cancellationToken)
        {

            if (branchId.HasValue && branchId.Value == Guid.Empty)
            {
                return BadRequest(new
                {
                    Message = "BranchId must be a valid identifier."
                });
            }

            var query = new GetTodayClassSessionsQuery(branchId);

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
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateClassSession(
        [FromBody] CreateClassSessionRequest request,
        CancellationToken cancellationToken)
        {
            

            var command = new CreateClassSessionCommand(
                request.ClassName,
                request.BranchId,
                request.StudioId,
                request.TrainerId,
                request.SessionDate,
                request.StartTime,
                request.DurationMinutes,
                request.CapacityLimit,
                request.Description);

            var result = await _sender.Send(
                command,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Created(
                    $"/api/class-sessions/{result.Value}",
                    result.Value);
            }

            return StatusCode(
                (int)result.Error.StatusCode,
                result.Error);
        }




        [HttpPost("{sessionId:guid}/bookings")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateBooking(
       Guid sessionId,
       [FromBody] CreateBookingRequest request,
       CancellationToken cancellationToken)
        {
            if (sessionId == Guid.Empty)
            {
                return BadRequest(new
                {
                    Message = "SessionId is required."
                });
            }

           

            var command = new CreateBookingCommand(
                sessionId,
                request.MemberId,
                request.TrainerNotes);

            var result = await _sender.Send(
                command,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Created(
                    $"/api/class-sessions/{sessionId}/bookings/{result.Value}",
                    result.Value);
            }

            return StatusCode(
                (int)result.Error.StatusCode,
                result.Error);
        }
    }
    }
