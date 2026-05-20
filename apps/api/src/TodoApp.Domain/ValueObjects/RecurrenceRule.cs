using TodoApp.Domain.Enums;

namespace TodoApp.Domain.ValueObjects;

public sealed record RecurrenceRule(
    RecurrenceFrequency Frequency,
    int Interval,
    DateTime? EndDateUtc = null,
    int? Count = null)
{
    public DateTime Next(DateTime fromUtc)
    {
        var step = Math.Max(1, Interval);
        return Frequency switch
        {
            RecurrenceFrequency.Daily => fromUtc.AddDays(step),
            RecurrenceFrequency.Weekly => fromUtc.AddDays(7 * step),
            RecurrenceFrequency.Monthly => fromUtc.AddMonths(step),
            _ => throw new InvalidOperationException($"Unknown frequency {Frequency}")
        };
    }
}
