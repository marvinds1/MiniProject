using MediatR;

namespace Application.Features.TodoDetail.Commands
{
    public class DeleteTodoDetailCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
