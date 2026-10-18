using TodoApi.DTOs.Tasks;

namespace TodoApi.Services.Tasks;

public interface ITodoTaskService
{
    Task<IReadOnlyList<TodoTaskResponse>> GetAllAsync(CancellationToken cancellationToken);
}