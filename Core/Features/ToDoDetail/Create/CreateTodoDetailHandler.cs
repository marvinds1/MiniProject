using Core.Interface.Service.TodoDetail;
using MediatR;
using Persistence.Models;

namespace Core.Features.TodoDetail
{
    public class CreateTodoDetailHandler : IRequestHandler<CreateTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _todoDetailService;

        public CreateTodoDetailHandler(ITodoDetailService todoDetailService)
        {
            _todoDetailService = todoDetailService;
        }

        public async Task<MainResponse> Handle(CreateTodoDetailQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return new MainResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "TodoDetail is required.",
                    Content = null
                };
            }

            await _todoDetailService.CreateTodoDetail(request);

            return new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = string.Empty,
                Content = request
            };
        }
    }
}
