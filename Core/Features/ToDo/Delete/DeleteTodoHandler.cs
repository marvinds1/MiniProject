using Core.Interface.Service.Todo;
using MediatR;

namespace Core.Features.Todo
{
    public class DeleteTodoHandler : IRequestHandler<DeleteTodoQuery, MainResponse>
    {
        private readonly ITodoService _todoService;

        public DeleteTodoHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }

        public async Task<MainResponse> Handle(DeleteTodoQuery request, CancellationToken cancellationToken)
        {
            var success = await _todoService.Delete(request.Id);
            if (!success)
            {
                return new MainResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Todo not found or could not be deleted",
                    Content = null
                };
            }

            return new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = "",
                Content = "Todo deleted successfully"
            };
        }
    }
}
