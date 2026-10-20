using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs.Tasks;

public sealed record CreateTodoTaskRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }

    [Required]
    public DateTime? DueDate { get; init; }
}