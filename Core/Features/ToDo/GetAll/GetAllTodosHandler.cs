using MediatR;
using Persistence.Models;
using Core.Interface.Service.Todo;

namespace Core.Features.Todo
{
    public class GetAllTodosHandler : IRequestHandler<GetAllTodosQuery, GetMainResponse>
    {
        private readonly ITodoService _todoService;

        public GetAllTodosHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }

        public async Task<GetMainResponse> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
        {
            var response = new GetMainResponse
            {
                IsSuccess = true,
                Content = await _todoService.GetAll()
            };

            return response;
        }
    }
}
