using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Text.Json;

namespace Application.Features.TodoDetail.Queries
{
    public class GetTodoDetailByIdQueryHandler : IRequestHandler<GetTodoDetailByIdQuery, TodoDetailResponse>
    {
        private readonly ApplicationDBContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetTodoDetailByIdQueryHandler> _logger;

        public GetTodoDetailByIdQueryHandler(ApplicationDBContext context, IDistributedCache cache, ILogger<GetTodoDetailByIdQueryHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<TodoDetailResponse> Handle(GetTodoDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"TodoDetails_{request.Id}";
            bool redis = true;

            try
            {
                var cachedTodoDetail = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedTodoDetail))
                {
                    return JsonSerializer.Deserialize<TodoDetailResponse>(cachedTodoDetail);
                }
            } catch (Exception ex)
            {
                redis = false;
                _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
            }

            

            var todoDetail = await _context.TodoDetails.FindAsync(new object[] { request.Id }, cancellationToken);
            if (todoDetail == null)
            {
                return null;
            }

            var todoDetailResponse = new TodoDetailResponse
            {
                Activity = todoDetail.Activity,
                Category = todoDetail.Category,
                DetailNote = todoDetail.DetailNote
            };

            if (redis)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                };

                var serializedTodo = JsonSerializer.Serialize(todoDetailResponse);
                try
                {
                    await _cache.SetStringAsync(cacheKey, serializedTodo, cacheOptions, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gagal mengakses Redis: {Message}", ex.Message);
                }
            }
            
            return todoDetailResponse;
        }
    }
}
