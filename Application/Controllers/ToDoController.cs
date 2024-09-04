using Application.Features.Todo.Commands;
using Application.Features.Todo.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponse>> GetTodoById(Guid id)
        {
            var query = new GetTodoByIdQuery { Id = id };
            var todo = await _mediator.Send(query);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoResponse>>> GetAllTodos()
        {
            var query = new GetAllTodosQuery();
            var todos = await _mediator.Send(query);
            return Ok(todos);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTodo([FromBody] TodoRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var command = new CreateTodoCommand { day = request.Day, note = request.Note };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTodo(Guid id, [FromBody] TodoRequest request)
        {
            if (id != request.TodoId)
            {
                return BadRequest();
            }

            var command = new UpdateTodoCommand { Todo = request };
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodo(Guid id)
        {
            var command = new DeleteTodoCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
