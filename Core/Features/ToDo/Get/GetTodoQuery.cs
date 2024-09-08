using MediatR;

namespace Core.Features.Todo
{
    public class GetTodoQuery : IRequest<MainResponse>
    {
        public Guid Id { get; set; }
    }
}
