namespace Persistence.Models
{
    public class TodoDetails1
    {
        public Guid TodoDetailId { get; set; }
        public Guid TodoId { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string DetailNote { get; set; } = string.Empty;

        public Todos? Todo { get; set; }
    }
}
