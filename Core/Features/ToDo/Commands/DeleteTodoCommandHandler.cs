using System;
using MediatR;
using Persistence.DatabaseContext;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Todo.Commands
{
    public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand>
    {
        private readonly ApplicationDBContext _context;

        public DeleteTodoCommandHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(request.Id);
            if (todo == null)
            {
                throw new KeyNotFoundException("Todo not found");
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        Task IRequestHandler<DeleteTodoCommand>.Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

