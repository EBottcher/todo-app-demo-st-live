using System.Collections.Concurrent;
using TodoApp.Application.Abstractions;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence;

public sealed class InMemoryTaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<Guid, TodoTask> _store = new();
    public IReadOnlyList<TodoTask> GetAll() => _store.Values.ToList();
    public TodoTask? GetById(Guid id) => _store.TryGetValue(id, out var v) ? v : null;
    public void Add(TodoTask task) => _store[task.Id] = task;
    public void Update(TodoTask task) => _store[task.Id] = task;
    public void Remove(Guid id) => _store.TryRemove(id, out _);
}

public sealed class InMemoryProjectRepository : IProjectRepository
{
    private readonly ConcurrentDictionary<Guid, Project> _store = new();
    public IReadOnlyList<Project> GetAll() => _store.Values.OrderBy(p => p.CreatedUtc).ToList();
    public Project? GetById(Guid id) => _store.TryGetValue(id, out var v) ? v : null;
    public void Add(Project project) => _store[project.Id] = project;
    public void Update(Project project) => _store[project.Id] = project;
    public void Remove(Guid id) => _store.TryRemove(id, out _);
}

public sealed class InMemoryLabelRepository : ILabelRepository
{
    private readonly ConcurrentDictionary<Guid, Label> _store = new();
    public IReadOnlyList<Label> GetAll() => _store.Values.OrderBy(l => l.Name).ToList();
    public Label? GetById(Guid id) => _store.TryGetValue(id, out var v) ? v : null;
    public void Add(Label label) => _store[label.Id] = label;
    public void Update(Label label) => _store[label.Id] = label;
    public void Remove(Guid id) => _store.TryRemove(id, out _);
}

public sealed class InMemoryActivityRepository : IActivityRepository
{
    private readonly ConcurrentQueue<ActivityEntry> _store = new();

    public IReadOnlyList<ActivityEntry> GetAll() =>
        _store.OrderByDescending(a => a.TimestampUtc).ToList();

    public IReadOnlyList<ActivityEntry> GetForEntity(Guid entityId, int take = 50) =>
        _store.Where(a => a.EntityId == entityId)
              .OrderByDescending(a => a.TimestampUtc)
              .Take(take)
              .ToList();

    public void Add(ActivityEntry entry) => _store.Enqueue(entry);
}
