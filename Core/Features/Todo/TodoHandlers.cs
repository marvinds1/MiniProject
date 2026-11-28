using Core.Interface.Service.Todo;
using MediatR;
using Persistence.Models;

namespace Core.Features.Todo
{
    public class GetAllTodosHandler : IRequestHandler<GetAllTodosQuery, GetMainResponse>
    {
        private readonly ITodoService _todos;
        public GetAllTodosHandler(ITodoService todos) => _todos = todos;

        public async Task<GetMainResponse> Handle(GetAllTodosQuery request, CancellationToken ct) =>
            GetMainResponse.From(await _todos.GetPage(request.UserId, request.Page, request.PageSize, ct));
    }

    public class GetTodoHandler : IRequestHandler<GetTodoQuery, MainResponse>
    {
        private readonly ITodoService _todos;
        public GetTodoHandler(ITodoService todos) => _todos = todos;

        public async Task<MainResponse> Handle(GetTodoQuery request, CancellationToken ct)
        {
            var todo = await _todos.Get(request.Id, request.UserId, ct);
            return todo is null ? MainResponse.Fail(404, "Todo not found") : MainResponse.Ok(todo);
        }
    }

    public class CreateTodoHandler : IRequestHandler<CreateTodoQuery, MainResponse>
    {
        private readonly ITodoService _todos;
        public CreateTodoHandler(ITodoService todos) => _todos = todos;

        public async Task<MainResponse> Handle(CreateTodoQuery request, CancellationToken ct) =>
            MainResponse.Ok(await _todos.Create(request.UserId, request.Day, request.Note, ct), 201);
    }

    public class UpdateTodoHandler : IRequestHandler<UpdateTodoQuery, MainResponse>
    {
        private readonly ITodoService _todos;
        public UpdateTodoHandler(ITodoService todos) => _todos = todos;

        public async Task<MainResponse> Handle(UpdateTodoQuery request, CancellationToken ct)
        {
            var (result, todo) = await _todos.Update(request.TodoId, request.UserId, request.Day, request.Note, request.Version, ct);
            return result switch
            {
                WriteResult.Ok when todo is not null => MainResponse.Ok(todo),
                WriteResult.Conflict => MainResponse.Fail(409, "Todo was modified by another request; reload and retry"),
                _ => MainResponse.Fail(404, "Todo not found")
            };
        }
    }

    public class DeleteTodoHandler : IRequestHandler<DeleteTodoQuery, MainResponse>
    {
        private readonly ITodoService _todos;
        public DeleteTodoHandler(ITodoService todos) => _todos = todos;

        public async Task<MainResponse> Handle(DeleteTodoQuery request, CancellationToken ct) =>
            await _todos.Delete(request.Id, request.UserId, request.IsAdmin, ct)
                ? MainResponse.Ok("Todo deleted successfully")
                : MainResponse.Fail(404, "Todo not found");
    }
}
