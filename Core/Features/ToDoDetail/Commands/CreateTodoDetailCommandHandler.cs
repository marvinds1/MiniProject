using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.TodoDetail.Commands
{
    public class CreateTodoDetailCommandHandler : IRequestHandler<CreateTodoDetailCommand, TodoDetailResponse>
    {
        private readonly ApplicationDBContext _context;

        public CreateTodoDetailCommandHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<TodoDetailResponse> Handle(CreateTodoDetailCommand request, CancellationToken cancellationToken)
        {
            if (request.TodoDetail.Category != "Task" && request.TodoDetail.Category != "Daily Activity")
            {
                throw new ArgumentException("Category must be either 'Task' or 'Daily Activity'");
            }

            var todoDetail = new TodoDetail1
            {
                TodoDetailId = Guid.NewGuid(),
                TodoId = request.TodoDetail.TodoId,
                Activity = request.TodoDetail.Activity,
                Category = request.TodoDetail.Category,
                DetailNote = request.TodoDetail.DetailNote
            };

            _context.TodoDetails.Add(todoDetail);
            await _context.SaveChangesAsync(cancellationToken);

            return new TodoDetailResponse
            {
                Activity = todoDetail.Activity,
                Category = todoDetail.Category,
                DetailNote = todoDetail.DetailNote
            };
        }
    }
}
