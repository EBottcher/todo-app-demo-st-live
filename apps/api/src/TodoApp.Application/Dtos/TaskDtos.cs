using TodoApp.Domain.Enums;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.Dtos;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TodoStatus Status,
    TodoPriority Priority,
    Guid? ProjectId,
    IReadOnlyList<Guid> LabelIds,
    DateTime? DueDateUtc,
    DateTime? ReminderUtc,
    bool ReminderFired,
    RecurrenceRule? Recurrence,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    DateTime? CompletedUtc);

public sealed record CreateTaskDto(
    string Title,
    string? Description,
    TodoPriority Priority = TodoPriority.Medium,
    Guid? ProjectId = null,
    IReadOnlyList<Guid>? LabelIds = null,
    DateTime? DueDateUtc = null,
    DateTime? ReminderUtc = null,
    RecurrenceRule? Recurrence = null);

public sealed record UpdateTaskDto(
    string Title,
    string? Description,
    TodoStatus Status,
    TodoPriority Priority,
    Guid? ProjectId,
    IReadOnlyList<Guid>? LabelIds,
    DateTime? DueDateUtc,
    DateTime? ReminderUtc,
    RecurrenceRule? Recurrence);

public sealed record UpdateStatusDto(TodoStatus Status);
