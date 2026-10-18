using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs.Tasks;

namespace TodoApi.Services.Tasks;

public sealed class TodoTaskService(TodoDbContext dbContext) : ITodoTaskService
{
    public async Task<IReadOnlyList<TodoTaskResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.DueDate)
            .Select(task => new TodoTaskResponse(
                task.Id,
                task.Title,
                task.Description,
                task.IsCompleted,
                task.DueDate))
            .ToListAsync(cancellationToken);
    }
}