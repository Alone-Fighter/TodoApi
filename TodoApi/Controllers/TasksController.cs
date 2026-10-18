using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs.Tasks;
using TodoApi.Services.Tasks;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public sealed class TasksController(ITodoTaskService todoTaskService) : ControllerBase
{
    [HttpGet(Name = "GetAllTasks")]
    [ProducesResponseType(typeof(IReadOnlyList<TodoTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<TodoTaskResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tasks = await todoTaskService.GetAllAsync(cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}", Name = "GetTaskById")]
    [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoTaskResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await todoTaskService.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Task not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"No task was found with ID '{id}'.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TodoTaskResponse>> Create(
        CreateTodoTaskRequest request,
        CancellationToken cancellationToken)
    {
        var createdTask = await todoTaskService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
    }
}