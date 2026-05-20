using TodoApp.Application.Abstractions;
using TodoApp.Application.Dtos;
using TodoApp.Application.Mapping;
using TodoApp.Domain.Abstractions;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Services;

public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _repo;
    private readonly IActivityService _activity;
    private readonly IClock _clock;

    public ProjectService(IProjectRepository repo, IActivityService activity, IClock clock)
    {
        _repo = repo; _activity = activity; _clock = clock;
    }

    public IReadOnlyList<ProjectDto> List() => _repo.GetAll().Select(p => p.ToDto()).ToList();
    public ProjectDto? Get(Guid id) => _repo.GetById(id)?.ToDto();

    public ProjectDto Create(CreateProjectDto dto)
    {
        var p = new Project { Name = dto.Name, Color = dto.Color, CreatedUtc = _clock.UtcNow };
        _repo.Add(p);
        _activity.Record(nameof(Project), p.Id, ActivityAction.Created, $"Project '{p.Name}' created");
        return p.ToDto();
    }

    public ProjectDto? Update(Guid id, UpdateProjectDto dto)
    {
        var p = _repo.GetById(id);
        if (p is null) return null;
        p.Name = dto.Name;
        p.Color = dto.Color;
        _repo.Update(p);
        _activity.Record(nameof(Project), p.Id, ActivityAction.Updated, $"Project '{p.Name}' updated");
        return p.ToDto();
    }

    public bool Delete(Guid id)
    {
        var p = _repo.GetById(id);
        if (p is null) return false;
        _repo.Remove(id);
        _activity.Record(nameof(Project), id, ActivityAction.Deleted, $"Project '{p.Name}' deleted");
        return true;
    }
}

public sealed class LabelService : ILabelService
{
    private readonly ILabelRepository _repo;
    private readonly IActivityService _activity;

    public LabelService(ILabelRepository repo, IActivityService activity)
    {
        _repo = repo; _activity = activity;
    }

    public IReadOnlyList<LabelDto> List() => _repo.GetAll().Select(l => l.ToDto()).ToList();
    public LabelDto? Get(Guid id) => _repo.GetById(id)?.ToDto();

    public LabelDto Create(CreateLabelDto dto)
    {
        var l = new Label { Name = dto.Name, Color = dto.Color };
        _repo.Add(l);
        _activity.Record(nameof(Label), l.Id, ActivityAction.Created, $"Label '{l.Name}' created");
        return l.ToDto();
    }

    public LabelDto? Update(Guid id, UpdateLabelDto dto)
    {
        var l = _repo.GetById(id);
        if (l is null) return null;
        l.Name = dto.Name;
        l.Color = dto.Color;
        _repo.Update(l);
        _activity.Record(nameof(Label), l.Id, ActivityAction.Updated, $"Label '{l.Name}' updated");
        return l.ToDto();
    }

    public bool Delete(Guid id)
    {
        var l = _repo.GetById(id);
        if (l is null) return false;
        _repo.Remove(id);
        _activity.Record(nameof(Label), id, ActivityAction.Deleted, $"Label '{l.Name}' deleted");
        return true;
    }
}
