using TodoApp.Application.Abstractions;
using TodoApp.Application.Dtos;
using TodoApp.Application.Mapping;
using TodoApp.Domain.Abstractions;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Services;

public sealed class ActivityService : IActivityService
{
    private readonly IActivityRepository _repo;
    private readonly IClock _clock;

    public ActivityService(IActivityRepository repo, IClock clock)
    {
        _repo = repo;
        _clock = clock;
    }

    public IReadOnlyList<ActivityDto> List(Guid? entityId = null, int take = 100)
    {
        var entries = entityId is { } id
            ? _repo.GetForEntity(id, take)
            : _repo.GetAll().OrderByDescending(a => a.TimestampUtc).Take(take).ToList();
        return entries.Select(e => e.ToDto()).ToList();
    }

    public void Record(string entityType, Guid entityId, ActivityAction action, string? summary = null, string? changeJson = null)
    {
        _repo.Add(new ActivityEntry
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Actor = "system",
            TimestampUtc = _clock.UtcNow,
            Summary = summary,
            ChangeJson = changeJson
        });
    }
}
