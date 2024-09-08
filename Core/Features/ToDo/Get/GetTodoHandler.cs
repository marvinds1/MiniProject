using Core.Interface.Service.Todo;
using MediatR;

namespace Core.Features.Todo
{
    public class GetTodoHandler : IRequestHandler<GetTodoQuery, MainResponse>
    {
        private readonly ITodoService _todoService;

        public GetTodoHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }

        public async Task<MainResponse> Handle(GetTodoQuery request, CancellationToken cancellationToken)
        {
            var content = await _todoService.Get(request.Id);
            if (content == null)
            {
                return new MainResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Todo not found",
                    Content = null
                };
            }

            return new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = "",
                Content = content
            };
        }
    }
}
