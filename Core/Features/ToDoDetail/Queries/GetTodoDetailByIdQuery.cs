using MediatR;
using Persistence.Models;

namespace Application.Features.TodoDetail.Queries
{
    public class GetTodoDetailByIdQuery : IRequest<TodoDetailResponse>
    {
        public Guid Id { get; set; }
    }
}
