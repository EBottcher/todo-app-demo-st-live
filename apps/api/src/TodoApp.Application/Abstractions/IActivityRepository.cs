using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstractions;

public interface IActivityRepository
{
    IReadOnlyList<ActivityEntry> GetAll();
    IReadOnlyList<ActivityEntry> GetForEntity(Guid entityId, int take = 50);
    void Add(ActivityEntry entry);
}
