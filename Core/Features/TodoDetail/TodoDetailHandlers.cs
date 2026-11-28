using Core.Interface.Service.TodoDetail;
using MediatR;

namespace Core.Features.TodoDetail
{
    public class GetAllTodoDetailsHandler : IRequestHandler<GetAllTodoDetailsQuery, GetMainResponse>
    {
        private readonly ITodoDetailService _details;
        public GetAllTodoDetailsHandler(ITodoDetailService details) => _details = details;

        public async Task<GetMainResponse> Handle(GetAllTodoDetailsQuery request, CancellationToken ct) =>
            GetMainResponse.From(await _details.GetPage(request.UserId, request.Page, request.PageSize, ct));
    }

    public class GetTodoDetailHandler : IRequestHandler<GetTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _details;
        public GetTodoDetailHandler(ITodoDetailService details) => _details = details;

        public async Task<MainResponse> Handle(GetTodoDetailQuery request, CancellationToken ct)
        {
            var detail = await _details.Get(request.TodoDetailId, request.UserId, ct);
            return detail is null ? MainResponse.Fail(404, "TodoDetail not found") : MainResponse.Ok(detail);
        }
    }

    public class GetTodoDetailsByTodoHandler : IRequestHandler<GetTodoDetailsByTodoQuery, MainResponse>
    {
        private readonly ITodoDetailService _details;
        public GetTodoDetailsByTodoHandler(ITodoDetailService details) => _details = details;

        public async Task<MainResponse> Handle(GetTodoDetailsByTodoQuery request, CancellationToken ct)
        {
            var details = await _details.GetByTodo(request.TodoId, request.UserId, ct);
            return details is null ? MainResponse.Fail(404, "Todo not found") : MainResponse.Ok(details);
        }
    }

    public class CreateTodoDetailHandler : IRequestHandler<CreateTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _details;
        public CreateTodoDetailHandler(ITodoDetailService details) => _details = details;

        public async Task<MainResponse> Handle(CreateTodoDetailQuery request, CancellationToken ct)
        {
            var created = await _details.CreateMany(request.TodoId, request.UserId, request.Items, ct);
            return created is null ? MainResponse.Fail(404, "Todo not found") : MainResponse.Ok(created, 201);
        }
    }

    public class UpdateTodoDetailHandler : IRequestHandler<UpdateTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _details;
        public UpdateTodoDetailHandler(ITodoDetailService details) => _details = details;

        public async Task<MainResponse> Handle(UpdateTodoDetailQuery request, CancellationToken ct)
        {
            var updated = await _details.Update(request.TodoDetailId, request.UserId, request.Values, ct);
            return updated is null ? MainResponse.Fail(404, "TodoDetail not found") : MainResponse.Ok(updated);
        }
    }

    public class DeleteTodoDetailHandler : IRequestHandler<DeleteTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _details;
        public DeleteTodoDetailHandler(ITodoDetailService details) => _details = details;

        public async Task<MainResponse> Handle(DeleteTodoDetailQuery request, CancellationToken ct) =>
            await _details.Delete(request.TodoDetailId, request.UserId, request.IsAdmin, ct)
                ? MainResponse.Ok("TodoDetail deleted successfully")
                : MainResponse.Fail(404, "TodoDetail not found");
    }
}
