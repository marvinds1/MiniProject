using MediatR;
using Persistence.Models;

namespace Core.Features.Todo
{
    public class UpdateTodoQuery : IRequest<MainResponse>
    {
        public Guid TodoId { get; set; }
        public string day { get; set; }
        public string note { get; set; }
    }
}
