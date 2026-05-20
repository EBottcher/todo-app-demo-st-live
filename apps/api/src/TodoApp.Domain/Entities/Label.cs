namespace TodoApp.Domain.Entities;

public sealed class Label
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#64748b";
}
