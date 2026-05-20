using TodoApp.Application.Dtos;
using DomainTaskStatus = TodoApp.Domain.Enums.TodoStatus;

namespace TodoApp.Application.Services;

public interface ITaskService
{
    IReadOnlyList<TaskDto> List(Guid? projectId = null, Guid? labelId = null, DomainTaskStatus? status = null);
    TaskDto? Get(Guid id);
    TaskDto Create(CreateTaskDto dto);
    TaskDto? Update(Guid id, UpdateTaskDto dto);
    TaskDto? UpdateStatus(Guid id, DomainTaskStatus status);
    /// <summary>Marks the task done; if recurring, also creates the next occurrence.</summary>
    /// <returns>Tuple of (completed task, optional next occurrence) or null if id not found.</returns>
    (TaskDto Completed, TaskDto? Next)? Complete(Guid id);
    bool Delete(Guid id);
}

public interface ISmartFilterService
{
    IReadOnlyList<TaskDto> Today();
    IReadOnlyList<TaskDto> Overdue();
    IReadOnlyList<TaskDto> Upcoming();
}
