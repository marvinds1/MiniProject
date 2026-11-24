namespace Persistence.Models
{
    public class Todos
    {
        public Guid TodoId { get; set; }
        public string? UserId { get; set; }
        public string day { get; set; } = string.Empty;
        public DateTime todayDate { get; set; }
        public string note { get; set; } = string.Empty;
        public int detailCount { get; set; }
        public int Version { get; set; }

        public User? User { get; set; }
        public ICollection<TodoDetails1>? TodoDetails { get; set; }
    }
}
