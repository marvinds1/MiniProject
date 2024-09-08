using MediatR;
using Persistence.Models;

namespace Core.Features.Todo
{
    public class GetAllTodosQuery : IRequest<GetMainResponse>
    {
    }
}
