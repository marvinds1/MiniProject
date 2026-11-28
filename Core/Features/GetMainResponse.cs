using Persistence.Models;

namespace Core.Features
{
    public class GetMainResponse
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<object>? Content { get; set; }
        public Pagination? Pagination { get; set; }

        public static GetMainResponse From<T>(PagedResult<T> page) where T : class => new()
        {
            IsSuccess = true,
            Content = page.Items,
            Pagination = new Pagination
            {
                CurrentPage = page.Page,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            }
        };
    }

    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
