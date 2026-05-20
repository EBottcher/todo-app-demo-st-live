using TodoApp.Application.Abstractions;
using TodoApp.Application.Dtos;
using TodoApp.Application.Mapping;
using TodoApp.Domain.Abstractions;
using DomainTaskStatus = TodoApp.Domain.Enums.TodoStatus;

namespace TodoApp.Application.Services;

public sealed class SmartFilterService : ISmartFilterService
{
    private readonly ITaskRepository _repo;
    private readonly IClock _clock;

    public SmartFilterService(ITaskRepository repo, IClock clock)
    {
        _repo = repo; _clock = clock;
    }

    public IReadOnlyList<TaskDto> Today()
    {
        var today = _clock.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        return _repo.GetAll()
            .Where(t => t.Status != DomainTaskStatus.Done
                        && t.DueDateUtc.HasValue
                        && t.DueDateUtc.Value >= today
                        && t.DueDateUtc.Value < tomorrow)
            .OrderBy(t => t.DueDateUtc)
            .Select(t => t.ToDto())
            .ToList();
    }

    public IReadOnlyList<TaskDto> Overdue()
    {
        var today = _clock.UtcNow.Date;
        return _repo.GetAll()
            .Where(t => t.Status != DomainTaskStatus.Done
                        && t.DueDateUtc.HasValue
                        && t.DueDateUtc.Value < today)
            .OrderBy(t => t.DueDateUtc)
            .Select(t => t.ToDto())
            .ToList();
    }

    public IReadOnlyList<TaskDto> Upcoming()
    {
        var tomorrow = _clock.UtcNow.Date.AddDays(1);
        var horizon = tomorrow.AddDays(7);
        return _repo.GetAll()
            .Where(t => t.Status != DomainTaskStatus.Done
                        && t.DueDateUtc.HasValue
                        && t.DueDateUtc.Value >= tomorrow
                        && t.DueDateUtc.Value < horizon)
            .OrderBy(t => t.DueDateUtc)
            .Select(t => t.ToDto())
            .ToList();
    }
}
