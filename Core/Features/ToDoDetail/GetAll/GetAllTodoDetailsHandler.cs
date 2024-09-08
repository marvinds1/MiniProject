using Core.Interface.Service.TodoDetail;
using MediatR;

namespace Core.Features.TodoDetail
{
    public class GetAllTodoDetailsHandler : IRequestHandler<GetAllTodoDetailsQuery, GetMainResponse>
    {
        private readonly ITodoDetailService _todoDetailService;

        public GetAllTodoDetailsHandler(ITodoDetailService todoDetailService)
        {
            _todoDetailService = todoDetailService;
        }

        public async Task<GetMainResponse> Handle(GetAllTodoDetailsQuery request, CancellationToken cancellationToken)
        {
            var allTodoDetails = await _todoDetailService.GetAllTodoDetails();

            return new GetMainResponse
            {
                IsSuccess = true,
                Content = allTodoDetails
            };
        }
    }
}
