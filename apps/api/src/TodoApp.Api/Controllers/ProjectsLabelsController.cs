using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Dtos;
using TodoApp.Application.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectService _svc;
    public ProjectsController(IProjectService svc) => _svc = svc;

    [HttpGet] public ActionResult<IReadOnlyList<ProjectDto>> List() => Ok(_svc.List());

    [HttpGet("{id:guid}")]
    public ActionResult<ProjectDto> Get(Guid id) =>
        _svc.Get(id) is { } p ? Ok(p) : NotFound();

    [HttpPost]
    public ActionResult<ProjectDto> Create([FromBody] CreateProjectDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return ValidationProblem("Name is required.");
        var created = _svc.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<ProjectDto> Update(Guid id, [FromBody] UpdateProjectDto dto) =>
        _svc.Update(id, dto) is { } p ? Ok(p) : NotFound();

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id) => _svc.Delete(id) ? NoContent() : NotFound();
}

[ApiController]
[Route("api/labels")]
public sealed class LabelsController : ControllerBase
{
    private readonly ILabelService _svc;
    public LabelsController(ILabelService svc) => _svc = svc;

    [HttpGet] public ActionResult<IReadOnlyList<LabelDto>> List() => Ok(_svc.List());

    [HttpGet("{id:guid}")]
    public ActionResult<LabelDto> Get(Guid id) =>
        _svc.Get(id) is { } l ? Ok(l) : NotFound();

    [HttpPost]
    public ActionResult<LabelDto> Create([FromBody] CreateLabelDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return ValidationProblem("Name is required.");
        var created = _svc.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<LabelDto> Update(Guid id, [FromBody] UpdateLabelDto dto) =>
        _svc.Update(id, dto) is { } l ? Ok(l) : NotFound();

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id) => _svc.Delete(id) ? NoContent() : NotFound();
}
