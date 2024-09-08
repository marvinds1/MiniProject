using Core.Interface.Service.Todo;
using MediatR;

namespace Core.Features.Todo
{
    public class CreateTodoHandler : IRequestHandler<CreateTodoQuery, MainResponse>
    {

        private readonly ITodoService _todoService;

        public CreateTodoHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }

        public async Task<MainResponse> Handle(CreateTodoQuery request, CancellationToken cancellationToken)
        {
            var content = await _todoService.Create(request);
            var resp = new MainResponse
            {
                IsSuccess = true,
                ErrorMessage = "",
                Content = content
            };

            return resp;
        }
    }
}