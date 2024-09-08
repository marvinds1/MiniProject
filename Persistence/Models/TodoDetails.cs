using System;
namespace Persistence.Models
{
    public class TodoDetails1
    {
        public Guid TodoDetailId { get; set; }
        public string Activity { get; set; }
        public string Category { get; set; }
        public string DetailNote { get; set; }

        public Todos Todo { get; set; }
    }

}

