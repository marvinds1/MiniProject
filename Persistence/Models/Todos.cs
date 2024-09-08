using System;
namespace Persistence.Models
{
    public class Todos
    {
        public Guid TodoId { get; set; }
        public string day { get; set; }
        public DateTime todayDate { get; set; }
        public string note { get; set; }
        public int detailCount { get; set; }

        public List<TodoDetails1>? TodoDetails { get; set; }
    }

}

