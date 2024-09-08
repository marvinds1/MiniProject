using Core.Features.TodoDetail;
using Core.Interface.Service.TodoDetail;
using Core.Services;
using Persistence.Models;
using Persistence.Repositories.ToDoDetail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interface.Service.TodoDetail
{
    public class TodoDetailService : ITodoDetailService
    {
        private readonly ITodoDetailRepository _todoDetailRepository;
        private readonly RedisService _redisService;
        private const string CacheKeyPrefix = "TodoDetail_";

        public TodoDetailService(ITodoDetailRepository todoDetailRepository, RedisService redisService)
        {
            _todoDetailRepository = todoDetailRepository;
            _redisService = redisService;
        }

        public async Task<TodoDetails1> GetTodoDetailById(Guid id)
        {
            string cacheKey = $"{CacheKeyPrefix}{id}";
            TodoDetails1 todoDetail = null;

            if (await _redisService.IsRedisAvailableAsync())
            {
                var cachedTodoDetail = await _redisService.GetFromRedisAsync<TodoDetails1>(cacheKey);
                if (cachedTodoDetail != null)
                {
                    return cachedTodoDetail.FirstOrDefault();
                }
            }

            todoDetail = await _todoDetailRepository.GetTodoDetailById(id);

            if (todoDetail != null && await _redisService.IsRedisAvailableAsync())
            {
                await _redisService.SaveToRedisAsync(cacheKey, new List<TodoDetails1> { todoDetail });
            }

            return todoDetail;
        }

        public async Task<IEnumerable<TodoWithDetails>> GetAllTodoDetails()
        {
            string cacheKey = $"{CacheKeyPrefix}All";
            IEnumerable<TodoWithDetails> allTodoDetails = null;

            if (await _redisService.IsRedisAvailableAsync())
            {
                var cachedTodoDetails = await _redisService.GetFromRedisAsync<TodoWithDetails>(cacheKey);
                if (cachedTodoDetails != null)
                {
                    return cachedTodoDetails;
                }
            }

            allTodoDetails = await _todoDetailRepository.GetAllTodoDetails();

            if (allTodoDetails != null && await _redisService.IsRedisAvailableAsync())
            {
                await _redisService.SaveToRedisAsync(cacheKey, allTodoDetails);
            }

            return allTodoDetails;
        }

        public async Task CreateTodoDetail(CreateTodoDetailQuery todoDetail)
        {
            await _todoDetailRepository.CreateTodoDetail(new TodoDetails1
            {
                TodoDetailId = new Guid(),
                TodoId = todoDetail.TodoId,
                Activity = todoDetail.Activity,
                DetailNote = todoDetail.DetailNote,
                Category = todoDetail.Category
            });

            string cacheKey = $"{CacheKeyPrefix}All";

            // Cek ketersediaan Redis
            if (await _redisService.IsRedisAvailableAsync())
            {
                var allTodoDetails = await _todoDetailRepository.GetAllTodoDetails();
                await _redisService.SaveToRedisAsync(cacheKey, allTodoDetails);
            }
        }

        public async Task UpdateTodoDetail(UpdateTodoDetailQuery todoDetail)
        {
            await _todoDetailRepository.UpdateTodoDetail(new TodoDetails1
            {
                TodoDetailId = todoDetail.TodoDetailId,
                Activity = todoDetail.Activity,
                DetailNote = todoDetail.DetailNote,
                Category = todoDetail.Category
            });

            string cacheKey = $"{CacheKeyPrefix}{todoDetail.TodoDetailId}";

            if (await _redisService.IsRedisAvailableAsync())
            {
                await _redisService.SaveToRedisAsync(cacheKey, new List<TodoDetails1>());
            }
        }

        public async Task DeleteTodoDetail(Guid id)
        {
            await _todoDetailRepository.DeleteTodoDetail(id);

            string cacheKey = $"{CacheKeyPrefix}{id}";
            string allCacheKey = $"{CacheKeyPrefix}All";

            if (await _redisService.IsRedisAvailableAsync())
            {
                await _redisService.RemoveFromRedisAsync(cacheKey);

                var allTodoDetails = await _todoDetailRepository.GetAllTodoDetails();
                await _redisService.SaveToRedisAsync(allCacheKey, allTodoDetails);
            }
        }
    }
}
