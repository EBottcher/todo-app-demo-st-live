using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstractions;

public interface ITaskRepository
{
    IReadOnlyList<TodoTask> GetAll();
    TodoTask? GetById(Guid id);
    void Add(TodoTask task);
    void Update(TodoTask task);
    void Remove(Guid id);
}
