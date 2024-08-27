using MediatR;
using Persistence.Models;
using System.Collections.Generic;

namespace Application.Features.TodoDetail.Queries
{
    public class GetAllTodoDetailsQuery : IRequest<IEnumerable<TodoWithDetailsResponse>>
    {
    }
}
