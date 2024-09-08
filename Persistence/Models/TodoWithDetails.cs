using System;

namespace Persistence.Models
{
    public class TodoWithDetails
    {
        public string Day { get; set; }
        public DateTime TodayDate { get; set; }
        public string Note { get; set; }
        public int DetailCount { get; set; }
        public List<TodoDetailResponse> Details { get; set; }
    }

    public class TodoDetailResponse
    {
        public string Activity { get; set; }
        public string Category { get; set; }
        public string DetailNote { get; set; }
    }

}