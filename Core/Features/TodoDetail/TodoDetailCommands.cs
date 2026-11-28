using MediatR;
using Persistence.Models;

namespace Core.Features.TodoDetail
{
    public record GetAllTodoDetailsQuery(string UserId, int Page, int PageSize) : IRequest<GetMainResponse>;
    public record GetTodoDetailQuery(Guid TodoDetailId, string UserId) : IRequest<MainResponse>;
    public record GetTodoDetailsByTodoQuery(Guid TodoId, string UserId) : IRequest<MainResponse>;
    public record CreateTodoDetailQuery(Guid TodoId, string UserId, IReadOnlyList<NewTodoDetail> Items) : IRequest<MainResponse>;
    public record UpdateTodoDetailQuery(Guid TodoDetailId, string UserId, NewTodoDetail Values) : IRequest<MainResponse>;
    public record DeleteTodoDetailQuery(Guid TodoDetailId, string UserId, bool IsAdmin) : IRequest<MainResponse>;
}
