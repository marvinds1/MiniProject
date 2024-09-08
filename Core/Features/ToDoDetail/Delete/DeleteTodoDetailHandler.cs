using Core.Interface.Service.TodoDetail;
using MediatR;

namespace Core.Features.TodoDetail
{
    public class DeleteTodoDetailHandler : IRequestHandler<DeleteTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _todoDetailService;

        public DeleteTodoDetailHandler(ITodoDetailService todoDetailService)
        {
            _todoDetailService = todoDetailService;
        }

        public async Task<MainResponse> Handle(DeleteTodoDetailQuery request, CancellationToken cancellationToken)
        {
            await _todoDetailService.DeleteTodoDetail(request.TodoDetailId);

            return new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = string.Empty,
                Content = "TodoDetail deleted successfully"
            };
        }
    }
}
