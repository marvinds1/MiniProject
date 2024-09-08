using MediatR;
using Persistence.Models;

namespace Core.Features.TodoDetail
{
    public class CreateTodoDetailQuery : IRequest<MainResponse>
    {
        public Guid TodoId { get; set; }
        public string Activity { get; set; }
        public string Category { get; set; }
        public string DetailNote { get; set; }
    }
}
