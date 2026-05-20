using System.Text.Json;
using TodoApp.Application.Abstractions;
using TodoApp.Application.Dtos;
using TodoApp.Application.Mapping;
using TodoApp.Domain.Abstractions;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using DomainTaskStatus = TodoApp.Domain.Enums.TodoStatus;

namespace TodoApp.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly IActivityService _activity;
    private readonly IClock _clock;

    public TaskService(ITaskRepository repo, IActivityService activity, IClock clock)
    {
        _repo = repo; _activity = activity; _clock = clock;
    }

    public IReadOnlyList<TaskDto> List(Guid? projectId = null, Guid? labelId = null, DomainTaskStatus? status = null)
    {
        IEnumerable<TodoTask> q = _repo.GetAll();
        if (projectId is { } pid) q = q.Where(t => t.ProjectId == pid);
        if (labelId is { } lid) q = q.Where(t => t.LabelIds.Contains(lid));
        if (status is { } s) q = q.Where(t => t.Status == s);
        return q.OrderByDescending(t => t.CreatedUtc).Select(t => t.ToDto()).ToList();
    }

    public TaskDto? Get(Guid id) => _repo.GetById(id)?.ToDto();

    public TaskDto Create(CreateTaskDto dto)
    {
        var now = _clock.UtcNow;
        var t = new TodoTask
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            ProjectId = dto.ProjectId,
            LabelIds = dto.LabelIds?.ToList() ?? new(),
            DueDateUtc = dto.DueDateUtc,
            ReminderUtc = dto.ReminderUtc,
            Recurrence = dto.Recurrence,
            CreatedUtc = now,
            UpdatedUtc = now
        };
        _repo.Add(t);
        _activity.Record(nameof(TodoTask), t.Id, ActivityAction.Created, $"Task '{t.Title}' created");
        return t.ToDto();
    }

    public TaskDto? Update(Guid id, UpdateTaskDto dto)
    {
        var t = _repo.GetById(id);
        if (t is null) return null;

        var prevStatus = t.Status;
        t.Title = dto.Title;
        t.Description = dto.Description;
        t.Status = dto.Status;
        t.Priority = dto.Priority;
        t.ProjectId = dto.ProjectId;
        t.LabelIds = dto.LabelIds?.ToList() ?? new();
        t.DueDateUtc = dto.DueDateUtc;
        if (dto.ReminderUtc != t.ReminderUtc)
        {
            t.ReminderUtc = dto.ReminderUtc;
            t.ReminderFired = false;
        }
        t.Recurrence = dto.Recurrence;
        t.UpdatedUtc = _clock.UtcNow;
        if (dto.Status == DomainTaskStatus.Done && t.CompletedUtc is null)
            t.CompletedUtc = _clock.UtcNow;
        if (dto.Status != DomainTaskStatus.Done)
            t.CompletedUtc = null;

        _repo.Update(t);
        _activity.Record(nameof(TodoTask), t.Id, ActivityAction.Updated, $"Task '{t.Title}' updated");
        if (prevStatus != t.Status)
            _activity.Record(nameof(TodoTask), t.Id, ActivityAction.StatusChanged, $"Status: {prevStatus} -> {t.Status}");
        return t.ToDto();
    }

    public TaskDto? UpdateStatus(Guid id, DomainTaskStatus status)
    {
        var t = _repo.GetById(id);
        if (t is null) return null;
        if (t.Status == status) return t.ToDto();

        var prev = t.Status;
        t.Status = status;
        t.UpdatedUtc = _clock.UtcNow;
        t.CompletedUtc = status == DomainTaskStatus.Done ? _clock.UtcNow : null;
        _repo.Update(t);
        _activity.Record(nameof(TodoTask), t.Id, ActivityAction.StatusChanged, $"Status: {prev} -> {status}");
        return t.ToDto();
    }

    public (TaskDto Completed, TaskDto? Next)? Complete(Guid id)
    {
        var t = _repo.GetById(id);
        if (t is null) return null;

        t.Status = DomainTaskStatus.Done;
        t.CompletedUtc = _clock.UtcNow;
        t.UpdatedUtc = _clock.UtcNow;
        _repo.Update(t);
        _activity.Record(nameof(TodoTask), t.Id, ActivityAction.Completed, $"Task '{t.Title}' completed");

        TaskDto? nextDto = null;
        if (t.Recurrence is { } rule)
        {
            var basis = t.DueDateUtc ?? _clock.UtcNow;
            var nextDue = rule.Next(basis);
            var withinWindow = (rule.EndDateUtc is null || nextDue <= rule.EndDateUtc);
            if (withinWindow)
            {
                var next = new TodoTask
                {
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    ProjectId = t.ProjectId,
                    LabelIds = t.LabelIds.ToList(),
                    DueDateUtc = nextDue,
                    ReminderUtc = t.ReminderUtc.HasValue && t.DueDateUtc.HasValue
                        ? nextDue - (t.DueDateUtc.Value - t.ReminderUtc.Value)
                        : null,
                    Recurrence = rule,
                    CreatedUtc = _clock.UtcNow,
                    UpdatedUtc = _clock.UtcNow
                };
                _repo.Add(next);
                _activity.Record(nameof(TodoTask), next.Id, ActivityAction.Recurred,
                    $"Recurred from {t.Id} due {nextDue:O}",
                    JsonSerializer.Serialize(new { sourceTaskId = t.Id, dueDateUtc = nextDue }));
                nextDto = next.ToDto();
            }
        }

        return (t.ToDto(), nextDto);
    }

    public bool Delete(Guid id)
    {
        var t = _repo.GetById(id);
        if (t is null) return false;
        _repo.Remove(id);
        _activity.Record(nameof(TodoTask), id, ActivityAction.Deleted, $"Task '{t.Title}' deleted");
        return true;
    }
}
