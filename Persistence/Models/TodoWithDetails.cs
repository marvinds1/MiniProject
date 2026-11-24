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

    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int TotalItems { get; init; }
    }
}
