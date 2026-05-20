namespace TodoApp.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#3b82f6";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
