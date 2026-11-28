using System.ComponentModel.DataAnnotations;

namespace Core.Features.TodoDetail
{
    public class TodoDetailItem
    {
        [Required, StringLength(200, MinimumLength = 1)]
        public string Activity { get; set; } = string.Empty;

        [Required, AllowedValues("Task", "Daily Activity", ErrorMessage = "Category must be 'Task' or 'Daily Activity'")]
        public string Category { get; set; } = string.Empty;

        [StringLength(1000)]
        public string DetailNote { get; set; } = string.Empty;
    }

    public class CreateTodoDetailsRequest
    {
        [Required]
        public Guid TodoId { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        public List<TodoDetailItem> Items { get; set; } = new();
    }
}
