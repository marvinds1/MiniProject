using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo.Queries
{
    public class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, IEnumerable<TodoResponse>>
    {
        private readonly ApplicationDBContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetAllTodosQueryHandler> _logger;
        

        public GetAllTodosQueryHandler(ApplicationDBContext context, IDistributedCache cache, ILogger<GetAllTodosQueryHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<TodoResponse>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "Todos";
            bool redis = true;

            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<IEnumerable<TodoResponse>>(cachedData);
                }
            } catch(Exception ex)
            {
                redis = false;
                _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
            }
            

            var todos = await _context.Todos.Select(todo => new TodoResponse
            {
                Day = todo.Day,
                TodayDate = todo.TodayDate,
                Note = todo.Note,
                DetailCount = todo.DetailCount
            }).ToListAsync(cancellationToken);

            if (redis)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                };

                var serializedData = JsonSerializer.Serialize(todos);
                try
                {
                    await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
                }
            }

            return todos;
        }
    }
}

