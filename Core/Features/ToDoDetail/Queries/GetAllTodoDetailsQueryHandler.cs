using Application.Features.Todo.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Persistence.DatabaseContext;
using Persistence.Models;
using Serilog.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.TodoDetail.Queries
{
    public class GetAllTodoDetailsQueryHandler : IRequestHandler<GetAllTodoDetailsQuery, IEnumerable<TodoWithDetailsResponse>>
    {
        private readonly ApplicationDBContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetAllTodoDetailsQueryHandler> _logger;

        public GetAllTodoDetailsQueryHandler(ApplicationDBContext context, IDistributedCache cache, ILogger<GetAllTodoDetailsQueryHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<TodoWithDetailsResponse>> Handle(GetAllTodoDetailsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "TodosDetails";
            bool redis = true;

            try
            {
                var cachedTodos = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedTodos))
                {
                    return JsonSerializer.Deserialize<IEnumerable<TodoWithDetailsResponse>>(cachedTodos);
                }
            } catch(Exception ex)
            {
                redis = false;
                _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
            }

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

            if (redis)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                };

                var serializedTodos = JsonSerializer.Serialize(todosWithDetails);
                try
                {
                    await _cache.SetStringAsync(cacheKey, serializedTodos, cacheOptions, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
                }
                
            }

            return todosWithDetails;
        }
    }

}
