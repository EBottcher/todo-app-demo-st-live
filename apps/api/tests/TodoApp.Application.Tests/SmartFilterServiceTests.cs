using TodoApp.Application.Dtos;
using TodoApp.Application.Services;
using TodoApp.Infrastructure.Persistence;
using Xunit;

namespace TodoApp.Application.Tests;

public class SmartFilterServiceTests
{
    [Fact]
    public void Partitions_tasks_by_today_overdue_upcoming()
    {
        var clock = new FakeClock();
        var taskRepo = new InMemoryTaskRepository();
        var activity = new ActivityService(new InMemoryActivityRepository(), clock);
        var taskSvc = new TaskService(taskRepo, activity, clock);
        var filters = new SmartFilterService(taskRepo, clock);

        var today = clock.UtcNow.Date;
        taskSvc.Create(new CreateTaskDto("today", null, DueDateUtc: today.AddHours(9)));
        taskSvc.Create(new CreateTaskDto("overdue", null, DueDateUtc: today.AddDays(-1)));
        taskSvc.Create(new CreateTaskDto("upcoming", null, DueDateUtc: today.AddDays(2)));
        taskSvc.Create(new CreateTaskDto("far future", null, DueDateUtc: today.AddDays(30)));

        Assert.Single(filters.Today());
        Assert.Single(filters.Overdue());
        Assert.Single(filters.Upcoming());
    }
}
