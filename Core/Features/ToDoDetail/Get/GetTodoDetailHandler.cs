using Core.Interface.Service.TodoDetail;
using MediatR;

namespace Core.Features.TodoDetail
{
    public class GetTodoDetailHandler : IRequestHandler<GetTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _todoDetailService;

        public GetTodoDetailHandler(ITodoDetailService todoDetailService)
        {
            _todoDetailService = todoDetailService;
        }

        public async Task<MainResponse> Handle(GetTodoDetailQuery request, CancellationToken cancellationToken)
        {
            var todoDetail = await _todoDetailService.GetTodoDetailById(request.TodoDetailId);

            return new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = string.Empty,
                Content = todoDetail
            };
        }
    }
}
