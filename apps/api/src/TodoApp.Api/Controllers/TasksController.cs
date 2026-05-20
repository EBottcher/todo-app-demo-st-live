using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Dtos;
using TodoApp.Application.Services;
using TodoApp.Domain.Enums;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;
    public TasksController(ITaskService tasks) => _tasks = tasks;

    [HttpGet]
    public ActionResult<IReadOnlyList<TaskDto>> List(
        [FromQuery] Guid? projectId,
        [FromQuery] Guid? labelId,
        [FromQuery] TodoStatus? status)
        => Ok(_tasks.List(projectId, labelId, status));

    [HttpGet("{id:guid}")]
    public ActionResult<TaskDto> Get(Guid id)
    {
        var t = _tasks.Get(id);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpPost]
    public ActionResult<TaskDto> Create([FromBody] CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return ValidationProblem("Title is required.");
        var created = _tasks.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<TaskDto> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var updated = _tasks.Update(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id:guid}/status")]
    public ActionResult<TaskDto> SetStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        var updated = _tasks.UpdateStatus(id, dto.Status);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/complete")]
    public ActionResult Complete(Guid id)
    {
        var result = _tasks.Complete(id);
        if (result is null) return NotFound();
        return Ok(new { completed = result.Value.Completed, next = result.Value.Next });
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
        => _tasks.Delete(id) ? NoContent() : NotFound();
}
