using Core.Features.TodoDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models;

namespace Application.Controllers
{
    [ApiController]
    [Authorize]
    public class TodoDetailController : BaseController
    {
        private readonly IMediator _mediator;

        public TodoDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTodoDetails()
        {
            var query = new GetAllTodoDetailsQuery();
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTodoDetail([FromRoute] Guid id)
        {
            var query = new GetTodoDetailQuery { TodoDetailId = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result);
        }
        

        [HttpPost]
        public async Task<IActionResult> CreateTodoDetail([FromBody] CreateTodoDetailQuery query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return CreatedAtAction(nameof(GetTodoDetail), new { id = ((TodoDetails1)result.Content).TodoDetailId }, result.Content);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTodoDetail(Guid id, [FromBody] UpdateTodoDetailQuery query)
        {
            if (id != query.TodoDetailId)
                return BadRequest("Mismatched TodoDetail ID.");

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteTodoDetail([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid ID.");

            var query = new DeleteTodoDetailQuery { TodoDetailId = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result);
        }
    }
}
