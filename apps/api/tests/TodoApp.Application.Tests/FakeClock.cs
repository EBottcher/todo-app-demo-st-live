using TodoApp.Domain.Abstractions;

namespace TodoApp.Application.Tests;

internal sealed class FakeClock : IClock
{
    public DateTime UtcNow { get; set; } = new(2026, 5, 20, 12, 0, 0, DateTimeKind.Utc);
}
