using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Application.Controllers
{
    [ApiController]
    public class TodoDetailController : BaseController
    {
        private readonly IMediator _mediator;

        public TodoDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //    [HttpGet("{id}")]
        //    public async Task<ActionResult<TodoDetailResponse>> GetTodoDetailById(Guid id)
        //    {
        //        var query = new GetTodoDetailByIdQuery { Id = id };
        //        var todoDetail = await _mediator.Send(query);
        //        if (todoDetail == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(todoDetail);
        //    }

        //    [HttpGet]
        //    public async Task<ActionResult<IEnumerable<TodoDetailResponse>>> GetAllTodoDetails()
        //    {
        //        var query = new GetAllTodoDetailsQuery();
        //        var todoDetails = await _mediator.Send(query);
        //        return Ok(todoDetails);
        //    }

        //    [Authorize]
        //    [HttpPost]
        //    public async Task<ActionResult> CreateTodoDetail([FromBody] TodoDetailRequest request)
        //    {
        //        if (request == null)
        //        {
        //            return BadRequest();
        //        }

        //        var command = new CreateTodoDetailCommand { TodoDetail = request };
        //        var result = await _mediator.Send(command);
        //        return CreatedAtAction(nameof(GetTodoDetailById), result);
        //    }

        //    [Authorize]
        //    [HttpPut("{id}")]
        //    public async Task<ActionResult> UpdateTodoDetail(Guid id, [FromBody] TodoDetailRequest request)
        //    {
        //        if (id != request.TodoDetailId)
        //        {
        //            return BadRequest();
        //        }

        //        var command = new UpdateTodoDetailCommand { TodoDetail = request };
        //        await _mediator.Send(command);
        //        return NoContent();
        //    }

        //    [Authorize(Roles = "admin")]
        //    [HttpDelete("{id}")]
        //    public async Task<ActionResult> DeleteTodoDetail(Guid id)
        //    {
        //        var command = new DeleteTodoDetailCommand { Id = id };
        //        await _mediator.Send(command);
        //        return NoContent();
        //    }
    }
}
