namespace TodoApi.DTOs.Tasks;

/// <summary>Represents a task returned by the API.</summary>
public sealed record TodoTaskResponse(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted,
    DateTime DueDate);