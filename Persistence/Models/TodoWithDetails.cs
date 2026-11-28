namespace Persistence.Models
{
    public class TodoWithDetails
    {
        public Guid TodoId { get; set; }
        public string Day { get; set; } = string.Empty;
        public DateTime TodayDate { get; set; }
        public string Note { get; set; } = string.Empty;
        public int DetailCount { get; set; }
        public List<TodoDetailResponse> Details { get; set; } = new();
    }

    public class TodoDetailResponse
    {
        public Guid TodoDetailId { get; set; }
        public Guid TodoId { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string DetailNote { get; set; } = string.Empty;
    }

    public record NewTodoDetail(string Activity, string Category, string DetailNote);

    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    }

    public enum WriteResult
    {
        Ok,
        NotFound,
        Conflict
    }
}
