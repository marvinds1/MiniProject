using MediatR;

namespace Core.Features.TodoDetail
{
    public class DeleteTodoDetailQuery : IRequest<MainResponse>
    {
        public Guid TodoDetailId { get; set; }
    }
}
