using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstractions;

public interface ILabelRepository
{
    IReadOnlyList<Label> GetAll();
    Label? GetById(Guid id);
    void Add(Label label);
    void Update(Label label);
    void Remove(Guid id);
}
