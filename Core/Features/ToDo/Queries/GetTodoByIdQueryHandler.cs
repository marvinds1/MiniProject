using System;
using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Todo.Queries
{
    public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoResponse>
    {
        private readonly ApplicationDBContext _context;

        public GetTodoByIdQueryHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<TodoResponse> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(request.Id);
            if (todo == null)
            {
                return null;
            }

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
