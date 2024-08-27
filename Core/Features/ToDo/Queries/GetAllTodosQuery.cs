using System;
using MediatR;
using System.Collections.Generic;
using Persistence.Models;

namespace Application.Features.Todo.Queries
{
    public class GetAllTodosQuery : IRequest<IEnumerable<TodoResponse>>
    {
    }
}
