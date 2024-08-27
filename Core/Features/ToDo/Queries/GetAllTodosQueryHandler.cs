using System;
using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Todo.Queries
{
    public class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, IEnumerable<TodoResponse>>
    {
        private readonly ApplicationDBContext _context;

        public GetAllTodosQueryHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoResponse>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
        {
            return await _context.Todos.Select(todo => new TodoResponse
            {
                Day = todo.Day,
                TodayDate = todo.TodayDate,
                Note = todo.Note,
                DetailCount = todo.DetailCount
            }).ToListAsync(cancellationToken);
        }
    }
}

