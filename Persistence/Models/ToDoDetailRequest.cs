using System;

namespace Persistence.Models
{
    public class TodoDetailRequest : TodoDetailResponse
    {
        public Guid TodoId { get; set; }
        public Guid TodoDetailId { get; set; }
    }
}
