using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstractions;

public interface IProjectRepository
{
    IReadOnlyList<Project> GetAll();
    Project? GetById(Guid id);
    void Add(Project project);
    void Update(Project project);
    void Remove(Guid id);
}
