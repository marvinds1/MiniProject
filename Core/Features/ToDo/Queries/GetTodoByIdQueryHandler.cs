using System;
using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo.Queries
{
    public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoResponse>
    {
        private readonly ApplicationDBContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetTodoByIdQueryHandler> _logger;

        public GetTodoByIdQueryHandler(ApplicationDBContext context, IDistributedCache cache, ILogger<GetTodoByIdQueryHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<TodoResponse> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
        {

            var cacheKey = $"Todo_{request.Id}";
            bool redis = true;

            try
            {
                var cachedTodo = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedTodo))
                {
                    return JsonSerializer.Deserialize<TodoResponse>(cachedTodo);
                }
            }
            catch (Exception ex)
            {
                redis = false;
                _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
            }

            var todo = await _context.Todos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (todo == null)
            {
                return null;
            }

            var todoResponse = new TodoResponse
            {
                Day = todo.Day,
                TodayDate = todo.TodayDate,
                Note = todo.Note,
                DetailCount = todo.DetailCount
            };

            if (redis)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                };

                var serializedTodo = JsonSerializer.Serialize(todoResponse);
                try
                {
                    await _cache.SetStringAsync(cacheKey, serializedTodo, cacheOptions, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
                }
            }

            return todoResponse;
        }
    }
}
