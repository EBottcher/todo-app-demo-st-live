using TodoApp.Application.Dtos;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Services;

public interface IActivityService
{
    IReadOnlyList<ActivityDto> List(Guid? entityId = null, int take = 100);
    void Record(string entityType, Guid entityId, ActivityAction action, string? summary = null, string? changeJson = null);
}
