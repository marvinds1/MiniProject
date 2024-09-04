using System;
using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Application.Features.Todo.Commands
{
    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoResponse>
    {
        private readonly ApplicationDBContext _context;

        public CreateTodoCommandHandler(ApplicationDBContext context, IDistributedCache cache)
        {
            _context = context;
        }

        public async Task<TodoResponse> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var todayDate = DateTime.Now;

            var todo = new Todo1
            {
                TodoId = Guid.NewGuid(),
                Day = request.day,
                TodayDate = todayDate,
                Note = request.note,
                DetailCount = 0
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
