namespace TodoApi.DTOs.Tasks;

public sealed record TodoTaskResponse(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted,
    DateTime DueDate);