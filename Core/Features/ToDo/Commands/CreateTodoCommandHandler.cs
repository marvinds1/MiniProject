using System;
using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Todo.Commands
{
    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoResponse>
    {
        private readonly ApplicationDBContext _context;

        public CreateTodoCommandHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<TodoResponse> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var todayDate = DateTime.Now;
            var detailCount = _context.TodoDetails.Count(td => td.TodoId == request.Todo.TodoId);

            var todo = new Todo1
            {
                TodoId = Guid.NewGuid(),
                Day = request.Todo.Day,
                TodayDate = todayDate,
                Note = request.Todo.Note,
                DetailCount = detailCount
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync(cancellationToken);

            return new TodoResponse
            {
                Day = todo.Day,
                TodayDate = todo.TodayDate,
                Note = todo.Note,
                DetailCount = todo.DetailCount
            };
        }
    }
}
