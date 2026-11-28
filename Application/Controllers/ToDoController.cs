using System.ComponentModel.DataAnnotations;
using Core.Features.Todo;
using Core.Features.TodoDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Authorize]
    public class TodoController : BaseController
    {
        private readonly IMediator _mediator;

        public TodoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodos([FromQuery, Range(1, 100000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10, CancellationToken ct = default) =>
            Ok(await _mediator.Send(new GetAllTodosQuery(CurrentUserId, page, pageSize), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new GetTodoQuery(id, CurrentUserId), ct));

        [HttpGet("{id:guid}/details")]
        public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new GetTodoDetailsByTodoQuery(id, CurrentUserId), ct));

        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoRequest request, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new CreateTodoQuery(CurrentUserId, request.day, request.note), ct));

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTodoRequest request, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new UpdateTodoQuery(id, CurrentUserId, request.day, request.note, request.version), ct));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTodo(Guid id, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new DeleteTodoQuery(id, CurrentUserId, IsAdmin), ct));
    }
}
