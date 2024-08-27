using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.TodoDetail.Queries
{
    public class GetAllTodoDetailsQueryHandler : IRequestHandler<GetAllTodoDetailsQuery, IEnumerable<TodoWithDetailsResponse>>
    {
        private readonly ApplicationDBContext _context;

        public GetAllTodoDetailsQueryHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoWithDetailsResponse>> Handle(GetAllTodoDetailsQuery request, CancellationToken cancellationToken)
        {
            var todosWithDetails = await _context.Todos
                .Select(todo => new TodoWithDetailsResponse
                {
                    Day = todo.Day,
                    TodayDate = todo.TodayDate,
                    Note = todo.Note,
                    DetailCount = _context.TodoDetails.Count(detail => detail.TodoId == todo.TodoId),
                    Details = _context.TodoDetails
                        .Where(detail => detail.TodoId == todo.TodoId)
                        .Select(detail => new TodoDetailResponse
                        {
                            Activity = detail.Activity,
                            Category = detail.Category,
                            DetailNote = detail.DetailNote
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return todosWithDetails;
        }
    }

}
