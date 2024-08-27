using System;
namespace Persistence.Models
{
    public class TodoDetail1
    {
        public Guid TodoDetailId { get; set; }
        public Guid TodoId { get; set; }
        public string Activity { get; set; }
        public string Category { get; set; }
        public string DetailNote { get; set; }

        public Todo1 Todo { get; set; }
    }

}

