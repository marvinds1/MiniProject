using Core.Interface.Service.Todo;
using MediatR;
using Persistence.Models;

namespace Core.Features.Todo
{
    public class UpdateTodoHandler : IRequestHandler<UpdateTodoQuery, MainResponse>
    {
        private readonly ITodoService _todoService;

        public UpdateTodoHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }

        public async Task<MainResponse> Handle(UpdateTodoQuery request, CancellationToken cancellationToken)
        {
            var content = await _todoService.Update(new Todos
            {
                TodoId = request.TodoId,
                day = request.day,
                todayDate = request.todayDate,
                note = request.note,
                detailCount = request.detailCount
            });

            return new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = "",
                Content = content
            };
        }
    }
}
