using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs.Tasks;

/// <summary>Payload for replacing the editable fields of a task.</summary>
public sealed record UpdateTodoTaskRequest
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