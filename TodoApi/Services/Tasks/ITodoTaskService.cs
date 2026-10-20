using TodoApi.DTOs.Tasks;

namespace TodoApi.Services.Tasks;

public interface ITodoTaskService
{
    Task<IReadOnlyList<TodoTaskResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<TodoTaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TodoTaskResponse> CreateAsync(CreateTodoTaskRequest request, CancellationToken cancellationToken);

    Task<TodoTaskResponse?> UpdateAsync(
        Guid id,
        UpdateTodoTaskRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}