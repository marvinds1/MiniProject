using MediatR;

namespace Core.Features.Todo
{
    public record GetAllTodosQuery(string UserId, int Page, int PageSize) : IRequest<GetMainResponse>;
    public record GetTodoQuery(Guid Id, string UserId) : IRequest<MainResponse>;
    public record CreateTodoQuery(string UserId, string Day, string Note) : IRequest<MainResponse>;
    public record UpdateTodoQuery(Guid TodoId, string UserId, string Day, string Note, int Version) : IRequest<MainResponse>;
    public record DeleteTodoQuery(Guid Id, string UserId, bool IsAdmin) : IRequest<MainResponse>;
}
