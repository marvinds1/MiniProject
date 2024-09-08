using Core.Features.Todo;
using Core.Interface.Service.Todo;
using Core.Services;
using Persistence.Repositories.ToDo;
using Persistence.Models;

namespace Core.Interface.Service.Todo
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly RedisService _redisService;

        public TodoService(ITodoRepository todoRepository, RedisService redisService)
        {
            _todoRepository = todoRepository;
            _redisService = redisService;
        }

        public async Task<Todo1> Create(CreateTodoQuery item)
        {
            var todos = await _todoRepository.CreateTodo(item.day, item.note);
            string cacheKey = "Todos";

            if (await _redisService.IsRedisAvailableAsync())
            {
                await GetAll(false);
                var allTodos = await _todoRepository.GetAllTodos();
                await _redisService.SaveToRedisAsync(cacheKey, allTodos);
            }

            return new Todo1
            {
                todayDate = todos.todayDate,
                day = todos.day,
                note = todos.note,
                detailCount = todos.detailCount
            };
        }

        public async Task<IEnumerable<Todo1>> GetAll(bool checkRedisAvailability = true)
        {
            string cacheKey = "Todos";
            bool isRedisAvailable = !checkRedisAvailability || await _redisService.IsRedisAvailableAsync();
            IEnumerable<Todo1> todos;

            if (isRedisAvailable)
            {
                var cachedTodos = await _redisService.GetFromRedisAsync<Todo1>(cacheKey);
                if (cachedTodos != null)
                {
                    return cachedTodos;
                }
            }

            todos = await _todoRepository.GetAllTodos();

            if (isRedisAvailable)
            {
                await _redisService.SaveToRedisAsync(cacheKey, todos);
            }

            return todos;
        }

        public async Task<Todo1> Get(Guid id)
        {
            string cacheKey = $"Todo_{id}";

            if (await _redisService.IsRedisAvailableAsync())
            {
                var cachedTodo = await _redisService.GetFromRedisAsync<Todo1>(cacheKey);
                if (cachedTodo != null && cachedTodo.Any())
                {
                    return cachedTodo.First();
                }
            }

            var todo = await _todoRepository.GetTodoById(id);

            if (todo != null && await _redisService.IsRedisAvailableAsync())
            {
                await _redisService.SaveToRedisAsync(cacheKey, new List<Todo1> { todo });
            }

            return todo;
        }

        public async Task<Todo1> Update(Todos item)
        {
            var updatedTodo = await _todoRepository.UpdateTodo(item);
            string cacheKey = $"Todo_{item.TodoId}";

            if (await _redisService.IsRedisAvailableAsync())
            {
                await _redisService.SaveToRedisAsync(cacheKey, new List<Todo1> { updatedTodo });
                var allTodos = await _todoRepository.GetAllTodos();
                await _redisService.SaveToRedisAsync("Todos", allTodos);
            }

            return updatedTodo;
        }

        public async Task<bool> Delete(Guid id)
        {
            var result = await _todoRepository.DeleteTodoById(id);
            string cacheKey = $"Todo_{id}";

            if (result && await _redisService.IsRedisAvailableAsync())
            {
                await _redisService.RemoveFromRedisAsync(cacheKey);
                var allTodos = await _todoRepository.GetAllTodos();
                await _redisService.SaveToRedisAsync("Todos", allTodos);
            }

            return result;
        }
    }
}
