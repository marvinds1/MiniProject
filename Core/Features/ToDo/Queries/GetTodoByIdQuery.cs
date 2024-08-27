using System;
using MediatR;
using Persistence.Models;

namespace Application.Features.Todo.Queries
{
    public class GetTodoByIdQuery : IRequest<TodoResponse>
    {
        public Guid Id { get; set; }
    }
}


