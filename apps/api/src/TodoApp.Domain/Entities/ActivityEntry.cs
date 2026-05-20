using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Entities;

public sealed class ActivityEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public ActivityAction Action { get; set; }
    public string Actor { get; set; } = "system";
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string? ChangeJson { get; set; }
    public string? Summary { get; set; }
}
