using TodoApp.Application.Dtos;

namespace TodoApp.Application.Services;

public interface IProjectService
{
    IReadOnlyList<ProjectDto> List();
    ProjectDto? Get(Guid id);
    ProjectDto Create(CreateProjectDto dto);
    ProjectDto? Update(Guid id, UpdateProjectDto dto);
    bool Delete(Guid id);
}

public interface ILabelService
{
    IReadOnlyList<LabelDto> List();
    LabelDto? Get(Guid id);
    LabelDto Create(CreateLabelDto dto);
    LabelDto? Update(Guid id, UpdateLabelDto dto);
    bool Delete(Guid id);
}
