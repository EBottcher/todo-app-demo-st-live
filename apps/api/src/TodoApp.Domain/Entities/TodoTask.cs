using TodoApp.Domain.ValueObjects;
using DomainTaskStatus = TodoApp.Domain.Enums.TodoStatus;
using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Entities;

public sealed class TodoTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DomainTaskStatus Status { get; set; } = DomainTaskStatus.Todo;
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public Guid? ProjectId { get; set; }
    public List<Guid> LabelIds { get; set; } = new();
    public DateTime? DueDateUtc { get; set; }
    public DateTime? ReminderUtc { get; set; }
    public bool ReminderFired { get; set; }
    public RecurrenceRule? Recurrence { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedUtc { get; set; }
}
