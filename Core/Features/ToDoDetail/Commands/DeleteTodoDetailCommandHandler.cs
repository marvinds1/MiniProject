using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Persistence.DatabaseContext;

namespace Application.Features.TodoDetail.Commands
{
    public class DeleteTodoDetailCommandHandler : IRequestHandler<DeleteTodoDetailCommand>
    {
        private readonly ApplicationDBContext _context;

        public DeleteTodoDetailCommandHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteTodoDetailCommand request, CancellationToken cancellationToken)
        {
            var todoDetail = await _context.TodoDetails.FindAsync(request.Id);
            if (todoDetail == null)
            {
                throw new KeyNotFoundException("TodoDetail not found");
            }

            _context.TodoDetails.Remove(todoDetail);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        Task IRequestHandler<DeleteTodoDetailCommand>.Handle(DeleteTodoDetailCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
