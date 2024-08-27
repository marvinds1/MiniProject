using System;
using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Todo.Commands
{
    public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand>
    {
        private readonly ApplicationDBContext _context;

        public UpdateTodoCommandHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(request.Todo.TodoId);
            if (todo == null)
            {
                throw new KeyNotFoundException("Todo not found");
            }

            todo.Day = request.Todo.Day;
            todo.TodayDate = request.Todo.TodayDate;
            todo.Note = request.Todo.Note;
            todo.DetailCount = request.Todo.DetailCount;

            _context.Todos.Update(todo);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        Task IRequestHandler<UpdateTodoCommand>.Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

