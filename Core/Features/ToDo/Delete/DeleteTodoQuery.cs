using MediatR;

namespace Core.Features.Todo
{
    public class DeleteTodoQuery : IRequest<MainResponse>
    {
        public Guid Id { get; set; }
    }
}