using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Dtos;
using TodoApp.Application.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/tasks/filters")]
public sealed class FiltersController : ControllerBase
{
    private readonly ISmartFilterService _filters;
    public FiltersController(ISmartFilterService filters) => _filters = filters;

    [HttpGet("today")] public ActionResult<IReadOnlyList<TaskDto>> Today() => Ok(_filters.Today());
    [HttpGet("overdue")] public ActionResult<IReadOnlyList<TaskDto>> Overdue() => Ok(_filters.Overdue());
    [HttpGet("upcoming")] public ActionResult<IReadOnlyList<TaskDto>> Upcoming() => Ok(_filters.Upcoming());
}

[ApiController]
[Route("api/activity")]
public sealed class ActivityController : ControllerBase
{
    private readonly IActivityService _activity;
    public ActivityController(IActivityService activity) => _activity = activity;

    [HttpGet]
    public ActionResult<IReadOnlyList<ActivityDto>> List(
        [FromQuery] Guid? entityId,
        [FromQuery] int take = 100)
        => Ok(_activity.List(entityId, take));
}

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet] public ActionResult Get() => Ok(new { status = "ok", timeUtc = DateTime.UtcNow });
}
