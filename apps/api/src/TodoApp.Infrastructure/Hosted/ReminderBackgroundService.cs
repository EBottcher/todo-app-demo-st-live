using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Abstractions;
using TodoApp.Application.Services;
using TodoApp.Domain.Abstractions;
using TodoApp.Domain.Enums;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Hosted;

public sealed class ReminderBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
    private readonly ITaskRepository _tasks;
    private readonly IActivityService _activity;
    private readonly IClock _clock;
    private readonly ILogger<ReminderBackgroundService> _logger;

    public ReminderBackgroundService(
        ITaskRepository tasks,
        IActivityService activity,
        IClock clock,
        ILogger<ReminderBackgroundService> logger)
    {
        _tasks = tasks; _activity = activity; _clock = clock; _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ScanOnce();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reminder scan failed");
            }

            try { await Task.Delay(PollInterval, stoppingToken); }
            catch (TaskCanceledException) { break; }
        }
    }

    private void ScanOnce()
    {
        var now = _clock.UtcNow;
        foreach (var t in _tasks.GetAll())
        {
            if (t.ReminderUtc is { } when_ && !t.ReminderFired && when_ <= now)
            {
                t.ReminderFired = true;
                _tasks.Update(t);
                _activity.Record(
                    nameof(TodoTask),
                    t.Id,
                    ActivityAction.ReminderFired,
                    $"Reminder fired for '{t.Title}' at {when_:O}");
            }
        }
    }
}
