using TodoApp.Application.Dtos;
using TodoApp.Application.Services;
using TodoApp.Domain.Enums;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Persistence;
using Xunit;

namespace TodoApp.Application.Tests;

public class TaskServiceTests
{
    private static (TaskService svc, FakeClock clock, InMemoryTaskRepository repo, InMemoryActivityRepository activityRepo)
        Build()
    {
        var clock = new FakeClock();
        var taskRepo = new InMemoryTaskRepository();
        var activityRepo = new InMemoryActivityRepository();
        var activity = new ActivityService(activityRepo, clock);
        var svc = new TaskService(taskRepo, activity, clock);
        return (svc, clock, taskRepo, activityRepo);
    }

    [Fact]
    public void Create_persists_task_and_records_activity()
    {
        var (svc, _, repo, activityRepo) = Build();

        var dto = svc.Create(new CreateTaskDto("Test", null));

        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Single(repo.GetAll());
        Assert.Contains(activityRepo.GetAll(), a => a.Action == ActivityAction.Created);
    }

    [Fact]
    public void Complete_recurring_task_creates_next_occurrence()
    {
        var (svc, clock, repo, _) = Build();
        var due = clock.UtcNow.Date;
        var created = svc.Create(new CreateTaskDto(
            "Daily",
            null,
            TodoPriority.Medium,
            null, null,
            due,
            null,
            new RecurrenceRule(RecurrenceFrequency.Daily, 1)));

        var result = svc.Complete(created.Id);

        Assert.NotNull(result);
        Assert.NotNull(result!.Value.Next);
        Assert.Equal(TodoStatus.Done, result.Value.Completed.Status);
        Assert.Equal(due.AddDays(1), result.Value.Next!.DueDateUtc);
        Assert.Equal(2, repo.GetAll().Count);
    }

    [Fact]
    public void Complete_non_recurring_task_returns_no_next()
    {
        var (svc, _, _, _) = Build();
        var t = svc.Create(new CreateTaskDto("Once", null));

        var result = svc.Complete(t.Id);

        Assert.NotNull(result);
        Assert.Null(result!.Value.Next);
    }

    [Fact]
    public void UpdateStatus_records_status_change()
    {
        var (svc, _, _, activity) = Build();
        var t = svc.Create(new CreateTaskDto("X", null));
        svc.UpdateStatus(t.Id, TodoStatus.InProgress);
        Assert.Contains(activity.GetAll(), a => a.Action == ActivityAction.StatusChanged);
    }
}
