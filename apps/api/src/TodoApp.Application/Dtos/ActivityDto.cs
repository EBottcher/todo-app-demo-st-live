using TodoApp.Domain.Enums;

namespace TodoApp.Application.Dtos;

public sealed record ActivityDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    ActivityAction Action,
    string Actor,
    DateTime TimestampUtc,
    string? Summary,
    string? ChangeJson);
