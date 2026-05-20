namespace TodoApp.Application.Dtos;

public sealed record ProjectDto(Guid Id, string Name, string Color, DateTime CreatedUtc);
public sealed record CreateProjectDto(string Name, string Color = "#3b82f6");
public sealed record UpdateProjectDto(string Name, string Color);

public sealed record LabelDto(Guid Id, string Name, string Color);
public sealed record CreateLabelDto(string Name, string Color = "#64748b");
public sealed record UpdateLabelDto(string Name, string Color);
