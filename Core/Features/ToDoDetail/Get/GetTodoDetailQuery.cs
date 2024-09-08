using MediatR;

namespace Core.Features.TodoDetail
{
    public class GetTodoDetailQuery : IRequest<MainResponse>
    {
        public Guid TodoDetailId { get; set; }
    }
}
