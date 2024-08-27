﻿using MediatR;
using Persistence.DatabaseContext;
using Persistence.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.TodoDetail.Commands
{
    public class UpdateTodoDetailCommandHandler : IRequestHandler<UpdateTodoDetailCommand>
    {
        private readonly ApplicationDBContext _context;

        public UpdateTodoDetailCommandHandler(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTodoDetailCommand request, CancellationToken cancellationToken)
        {
            var todoDetail = await _context.TodoDetails.FindAsync(request.TodoDetail.TodoDetailId);
            if (todoDetail == null)
            {
                throw new KeyNotFoundException("TodoDetail not found");
            }

            todoDetail.TodoId = request.TodoDetail.TodoId;
            todoDetail.Activity = request.TodoDetail.Activity;
            todoDetail.Category = request.TodoDetail.Category;
            todoDetail.DetailNote = request.TodoDetail.DetailNote;

            _context.TodoDetails.Update(todoDetail);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        Task IRequestHandler<UpdateTodoDetailCommand>.Handle(UpdateTodoDetailCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
