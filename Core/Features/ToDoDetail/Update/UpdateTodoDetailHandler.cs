using Core.Interface.Service.TodoDetail;
using MediatR;
using Persistence.Models;

namespace Core.Features.TodoDetail
{
    public class UpdateTodoDetailHandler : IRequestHandler<UpdateTodoDetailQuery, MainResponse>
    {
        private readonly ITodoDetailService _todoDetailService;

        public UpdateTodoDetailHandler(ITodoDetailService todoDetailService)
        {
            _todoDetailService = todoDetailService;
        }

        public async Task<MainResponse> Handle(UpdateTodoDetailQuery request, CancellationToken cancellationToken)
        {
            await _todoDetailService.UpdateTodoDetail(request);

            return new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = string.Empty,
                Content = request
            };
        }
    }
}
