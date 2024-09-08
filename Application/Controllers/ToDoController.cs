using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Features.Todo;

namespace Application.Controllers
{
    [ApiController]
    [Authorize]
    public class TodoController : BaseController
    {
        private readonly IMediator _mediator;

        public TodoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTodos()
        {
            var query = new GetAllTodosQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _mediator.Send(new GetTodoQuery { Id = id });
            if (!response.IsSuccess)
            {
                return NotFound(response.ErrorMessage);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoQuery request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var command = new CreateTodoQuery { day = request.day, note = request.note };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTodoQuery query)
        {
            var response = await _mediator.Send(query);
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response);
        }

        [HttpDelete]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteTodo([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid ID.");

            var query = new DeleteTodoQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result);
        }

    }
}
