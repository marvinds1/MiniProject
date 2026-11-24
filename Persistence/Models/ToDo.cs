namespace Persistence.Models
{
    public class Todo1
    {
        public Guid todoId { get; set; }
        public string day { get; set; } = string.Empty;
        public DateTime todayDate { get; set; }
        public string note { get; set; } = string.Empty;
        public int detailCount { get; set; }
        public int version { get; set; }
    }
}
