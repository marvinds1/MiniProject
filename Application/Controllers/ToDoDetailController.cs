using System.ComponentModel.DataAnnotations;
using Core.Features.TodoDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models;

namespace Application.Controllers
{
    [Authorize]
    public class TodoDetailController : BaseController
    {
        private readonly IMediator _mediator;

        public TodoDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodoDetails([FromQuery, Range(1, 100000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10, CancellationToken ct = default) =>
            Ok(await _mediator.Send(new GetAllTodoDetailsQuery(CurrentUserId, page, pageSize), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTodoDetail(Guid id, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new GetTodoDetailQuery(id, CurrentUserId), ct));

        [HttpPost]
        public async Task<IActionResult> CreateTodoDetails(CreateTodoDetailsRequest request, CancellationToken ct)
        {
            var items = request.Items.Select(i => new NewTodoDetail(i.Activity, i.Category, i.DetailNote)).ToList();
            return FromResponse(await _mediator.Send(new CreateTodoDetailQuery(request.TodoId, CurrentUserId, items), ct));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTodoDetail(Guid id, TodoDetailItem request, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new UpdateTodoDetailQuery(id, CurrentUserId, new NewTodoDetail(request.Activity, request.Category, request.DetailNote)), ct));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTodoDetail(Guid id, CancellationToken ct) =>
            FromResponse(await _mediator.Send(new DeleteTodoDetailQuery(id, CurrentUserId, IsAdmin), ct));
    }
}
