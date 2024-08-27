using System;
namespace Persistence.Models
{
    public class Todo1
    {
        public Guid TodoId { get; set; }
        public string Day { get; set; }
        public DateTime TodayDate { get; set; }
        public string Note { get; set; }
        public int DetailCount { get; set; }

        public ICollection<TodoDetail1> TodoDetails { get; set; }
    }

}

