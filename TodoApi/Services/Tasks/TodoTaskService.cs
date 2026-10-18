using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs.Tasks;
using TodoApi.Domain.Entities;

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

    public async Task<TodoTaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Id == id)
            .Select(task => new TodoTaskResponse(
                task.Id,
                task.Title,
                task.Description,
                task.IsCompleted,
                task.DueDate))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TodoTaskResponse> CreateAsync(
        CreateTodoTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = new TodoTask
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            DueDate = request.DueDate!.Value
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TodoTaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.IsCompleted,
            task.DueDate);
    }
}