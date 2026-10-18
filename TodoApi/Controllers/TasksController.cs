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
}