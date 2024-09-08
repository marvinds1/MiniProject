using MediatR;

namespace Core.Features.Todo
{
    public class CreateTodoQuery : IRequest<MainResponse>
    {
        public string day { get; set; }
        public string note { get; set; }
    }
}
