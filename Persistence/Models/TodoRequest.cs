using System;

namespace Persistence.Models
{
    public class TodoRequest : TodoResponse
    {
        public Guid TodoId { get; set; }
    }
}

