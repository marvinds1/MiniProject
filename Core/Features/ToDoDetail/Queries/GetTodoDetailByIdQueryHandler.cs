using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.TodoDetail.Queries
{
    public class GetTodoDetailByIdQueryHandler : IRequestHandler<GetTodoDetailByIdQuery, TodoDetailResponse>
    {
        private readonly ApplicationDBContext _context;

        public GetTodoDetailByIdQueryHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<TodoDetailResponse> Handle(GetTodoDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var todoDetail = await _context.TodoDetails.FindAsync(request.Id);
            if (todoDetail == null)
            {
                return null;
            }

            return new TodoDetailResponse
            {
                Activity = todoDetail.Activity,
                Category = todoDetail.Category,
                DetailNote = todoDetail.DetailNote
            };
        }
    }
}
