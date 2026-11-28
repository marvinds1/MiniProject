using System.ComponentModel.DataAnnotations;

namespace Core.Features.Todo
{
    public class CreateTodoRequest
    {
        [Required, StringLength(20, MinimumLength = 1)]
        public string day { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string note { get; set; } = string.Empty;
    }

    public class UpdateTodoRequest
    {
        [Required, StringLength(20, MinimumLength = 1)]
        public string day { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string note { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int version { get; set; }
    }
}
