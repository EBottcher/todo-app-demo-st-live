namespace TodoApp.Domain.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}
