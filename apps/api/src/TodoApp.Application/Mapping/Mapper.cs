using TodoApp.Application.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mapping;

internal static class Mapper
{
    public static TaskDto ToDto(this TodoTask t) => new(
        t.Id, t.Title, t.Description, t.Status, t.Priority, t.ProjectId,
        t.LabelIds.ToArray(), t.DueDateUtc, t.ReminderUtc, t.ReminderFired,
        t.Recurrence, t.CreatedUtc, t.UpdatedUtc, t.CompletedUtc);

    public static ProjectDto ToDto(this Project p) => new(p.Id, p.Name, p.Color, p.CreatedUtc);
    public static LabelDto ToDto(this Label l) => new(l.Id, l.Name, l.Color);

    public static ActivityDto ToDto(this ActivityEntry a) => new(
        a.Id, a.EntityType, a.EntityId, a.Action, a.Actor, a.TimestampUtc, a.Summary, a.ChangeJson);
}
