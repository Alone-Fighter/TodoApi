using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.DTOs.Tasks;
using TodoApi.Services.Tasks;

namespace TodoApi.Tests;

public sealed class TasksControllerTests
{
    private readonly Mock<ITodoTaskService> todoTaskService = new();

    [Fact]
    public async Task GetAll_ReturnsOkWithTasks()
    {
        var cancellationToken = CancellationToken.None;
        IReadOnlyList<TodoTaskResponse> tasks = [CreateTaskResponse()];
        todoTaskService
            .Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(tasks);
        var controller = CreateController();

        var action = await controller.GetAll(cancellationToken);

        var result = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Same(tasks, result.Value);
        todoTaskService.Verify(service => service.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenTaskExists_ReturnsOk()
    {
        var cancellationToken = CancellationToken.None;
        var task = CreateTaskResponse();
        todoTaskService
            .Setup(service => service.GetByIdAsync(task.Id, cancellationToken))
            .ReturnsAsync(task);
        var controller = CreateController();

        var action = await controller.GetById(task.Id, cancellationToken);

        var result = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Same(task, result.Value);
    }

    [Fact]
    public async Task GetById_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        todoTaskService
            .Setup(service => service.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoTaskResponse?)null);
        var controller = CreateController();

        var action = await controller.GetById(id, CancellationToken.None);

        var result = Assert.IsType<NotFoundObjectResult>(action.Result);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtGetById()
    {
        var cancellationToken = CancellationToken.None;
        var request = CreateRequest();
        var task = CreateTaskResponse();
        todoTaskService
            .Setup(service => service.CreateAsync(request, cancellationToken))
            .ReturnsAsync(task);
        var controller = CreateController();

        var action = await controller.Create(request, cancellationToken);

        var result = Assert.IsType<CreatedAtActionResult>(action.Result);
        Assert.Equal(nameof(TasksController.GetById), result.ActionName);
        Assert.Equal(task.Id, result.RouteValues!["id"]);
        Assert.Same(task, result.Value);
    }

    [Fact]
    public async Task Update_WhenTaskExists_ReturnsOk()
    {
        var cancellationToken = CancellationToken.None;
        var id = Guid.NewGuid();
        var request = CreateUpdateRequest();
        var task = CreateTaskResponse(id);
        todoTaskService
            .Setup(service => service.UpdateAsync(id, request, cancellationToken))
            .ReturnsAsync(task);
        var controller = CreateController();

        var action = await controller.Update(id, request, cancellationToken);

        var result = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Same(task, result.Value);
    }

    [Fact]
    public async Task Update_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var request = CreateUpdateRequest();
        todoTaskService
            .Setup(service => service.UpdateAsync(id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoTaskResponse?)null);
        var controller = CreateController();

        var action = await controller.Update(id, request, CancellationToken.None);

        var result = Assert.IsType<NotFoundObjectResult>(action.Result);
        Assert.IsType<ProblemDetails>(result.Value);
    }

    [Fact]
    public async Task Delete_WhenTaskExists_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        todoTaskService
            .Setup(service => service.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var controller = CreateController();

        var result = await controller.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        todoTaskService
            .Setup(service => service.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = CreateController();

        var result = await controller.Delete(id, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.IsType<ProblemDetails>(notFound.Value);
    }

    private TasksController CreateController()
    {
        var controller = new TasksController(todoTaskService.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/tasks";
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    private static TodoTaskResponse CreateTaskResponse(Guid? id = null)
    {
        return new TodoTaskResponse(
            id ?? Guid.NewGuid(),
            "Prepare API demo",
            "Task description",
            false,
            new DateTime(2026, 10, 25, 12, 0, 0, DateTimeKind.Utc));
    }

    private static CreateTodoTaskRequest CreateRequest()
    {
        return new CreateTodoTaskRequest
        {
            Title = "Prepare API demo",
            Description = "Task description",
            DueDate = new DateTime(2026, 10, 25, 12, 0, 0, DateTimeKind.Utc)
        };
    }

    private static UpdateTodoTaskRequest CreateUpdateRequest()
    {
        return new UpdateTodoTaskRequest
        {
            Title = "Prepare API demo",
            Description = "Updated task description",
            DueDate = new DateTime(2026, 10, 26, 12, 0, 0, DateTimeKind.Utc)
        };
    }
}

public sealed class CreateTodoTaskRequestValidationTests
{
    [Fact]
    public void MissingTitleAndDueDate_ReturnsValidationErrors()
    {
        var request = new CreateTodoTaskRequest
        {
            Description = "Task description"
        };
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Title)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.DueDate)));
    }
}