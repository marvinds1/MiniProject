using MediatR;

namespace Core.Features.TodoDetail
{
    public class UpdateTodoDetailQuery : IRequest<MainResponse>
    {
        public Guid TodoDetailId { get; set; }
        public string Activity { get; set; }
        public string Category { get; set; }
        public string DetailNote { get; set; }
    }
}
